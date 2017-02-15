using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace tegaki_hack
{
    public enum EShapeCreatorFamily { None, Freehand, Line, Circle, Text }

    public partial class Editor
    {
        DPoint<Internal> size;
        DPoint<External> realsize;
        Transform<Internal, External> transform;
        List<IShape> drawnShapes, undrawnShapes, redoShapes;
        EShapeCreator eShapeCreator;
        ShapeCreatorSettings settings;
        AShapeCreator shapeCreator;
        Dictionary<EShapeCreatorFamily, EShapeCreator[]> shapeDictionary;
        Dictionary<EShapeCreatorFamily, int> shapes;
        EShapeCreatorFamily eShapeCreatorFamily;

        bool isUndoable => drawnShapes.Count > 0;
        bool isRedoable => redoShapes.Count > 0;
        bool isClearable => drawnShapes.Count > 0 || undrawnShapes.Count > 0 || redoShapes.Count > 0;

        void InitializeFirst()
        {
            shapeDictionary = new Dictionary<EShapeCreatorFamily, EShapeCreator[]>();
            shapeDictionary[EShapeCreatorFamily.Freehand]
                = new EShapeCreator[] { EShapeCreator.Freehand };
            shapeDictionary[EShapeCreatorFamily.Line]
                = new EShapeCreator[] { EShapeCreator.Line, EShapeCreator.Arc, EShapeCreator.Polyline };
            shapeDictionary[EShapeCreatorFamily.Circle]
                = new EShapeCreator[] { EShapeCreator.Oval, EShapeCreator.Rectangle, EShapeCreator.RegularPolygon, EShapeCreator.Polygon };
            shapeDictionary[EShapeCreatorFamily.Text]
                = new EShapeCreator[] { EShapeCreator.Text, EShapeCreator.FancyText };

            drawnShapes = new List<IShape>();
            undrawnShapes = application.savedShapes == null ? new List<IShape>() : application.savedShapes;
            redoShapes = new List<IShape>();

            settings = new ShapeCreatorSettings(() =>
            {
                redoShapes.Clear();
                SetRedoAbility(false);
                Redisplay();
            }, (shape) =>
            {
                if (shape != null)
                {
                    undrawnShapes.Add(shape);
                    SetAbilities();
                }
                Redisplay();
            });

            shapes = new Dictionary<EShapeCreatorFamily, int>();
            shapes[EShapeCreatorFamily.Freehand] = 0;
            shapes[EShapeCreatorFamily.Line] = 0;
            shapes[EShapeCreatorFamily.Circle] = 0;
            shapes[EShapeCreatorFamily.Text] = 0;
            eShapeCreatorFamily = EShapeCreatorFamily.Freehand;
            ChangeShapeCreator(EShapeCreator.Freehand);

            size = new DPoint<Internal>(30, 30);
        }

        void InitializeLast()
        {
            SetAbilities();
            SetActiveness();
            SetIcons();
        }

        void DealWithLayoutChange(DPoint<External> realsize)
        {
            this.realsize = realsize;
            transform.scale = realsize.dx / size.dx;

            ResetSecondCanvas();
            Redisplay();
        }

        static Exception NotUndoable()
        {
            return new InvalidOperationException("Editor Not Undoable!");
        }
        void Undo()
        {
            if (!isUndoable) throw NotUndoable();

            redoShapes.Add(drawnShapes.Pop());
            SetAbilities();
            ResetSecondCanvas();
            Redisplay();
        }
        static Exception NotRedoable()
        {
            return new InvalidOperationException("Editor Not Redoable!");
        }
        void Redo()
        {
            if (!isRedoable) throw NotRedoable();

            undrawnShapes.Add(redoShapes.Pop());
            SetAbilities();
            Redisplay();
        }
        static Exception NotClearable()
        {
            return new InvalidOperationException("Editor Not Clearable!");
        }
        void Clear()
        {
            if (!isClearable) throw NotClearable();

            SetUndoAbility(false);
            SetRedoAbility(false);
            SetClearAbility(false);
            drawnShapes.Clear();
            undrawnShapes.Clear();
            redoShapes.Clear();
            ResetSecondCanvas();
            Redisplay();
        }
        void SetAbilities()
        {
            SetUndoAbility(isUndoable);
            SetRedoAbility(isRedoable);
            SetClearAbility(isClearable);
        }

        void Touch(Touchevent touchevent, Point<External> p)
        {
            shapeCreator?.Touch(touchevent, p.Untransform(transform));
        }

        void ToggleShapeCreatorFamily(EShapeCreatorFamily eShapeCreatorFamily)
        {
            if(eShapeCreatorFamily == EShapeCreatorFamily.None)
            {
                ChangeShapeCreator(EShapeCreator.None);
            }
            else if (this.eShapeCreatorFamily == eShapeCreatorFamily)
            {
                shapes[eShapeCreatorFamily] =
                    (shapes[eShapeCreatorFamily] + 1)
                    % shapeDictionary[eShapeCreatorFamily].Length;
                ChangeShapeCreator(shapeDictionary[eShapeCreatorFamily][shapes[eShapeCreatorFamily]]);
            }
            else
            {
                this.eShapeCreatorFamily = eShapeCreatorFamily;
                ChangeShapeCreator(shapeDictionary[eShapeCreatorFamily][shapes[eShapeCreatorFamily]]);
            }
            SetActiveness();
            SetIcons();
        }
        void ChangeShapeCreator(EShapeCreator eShapeCreator)
        {
            shapeCreator?.Cleanup();
            this.eShapeCreator = eShapeCreator;
            shapeCreator = ShapeCreator.GetShapeCreator(eShapeCreator);
            if (shapeCreator != null) shapeCreator.settings = settings;
        }
        void SetShapeCreator(EShapeCreator eShapeCreator)
        {
            if (this.eShapeCreator != eShapeCreator) ChangeShapeCreator(eShapeCreator);
        }
        void ResetShapeCreator()
        {
            ChangeShapeCreator(eShapeCreator);
        }

        XDocument GetSvg()
        {
            var shapes = new List<IShape>();
            shapes.AddRange(drawnShapes);
            shapes.AddRange(undrawnShapes);
            var svg = new XElement(Util.SvgName("svg"),
                new XAttribute("viewbox", string.Format("0 0 {0}", realsize)));
            foreach (var shape in shapes)
            {
                svg.AddSvg(shape, transform);
            }
            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                svg);
        }

        void MoveUndrawnShapesToDrawn()
        {
            if (undrawnShapes.Count > 0)
            {
                if (drawnShapes.Count > 0)
                {
                    drawnShapes.AddRange(undrawnShapes);
                    undrawnShapes.Clear();
                }
                else
                {
                    drawnShapes = undrawnShapes;
                    undrawnShapes = new List<IShape>();
                }
            }
            SetAbilities();
        }
        void MoveDrawnShapesToUndrawn()
        {
            if (drawnShapes.Count > 0)
            {
                drawnShapes.AddRange(undrawnShapes);
                undrawnShapes = drawnShapes;
                drawnShapes = new List<IShape>();
            }
            SetAbilities();
        }
        IShape Grid()
        {
            var shapes = new List<IShape>();
            var paint = new Paint(Color.Rgba(0xd3d3d3ff), new SizeEither(1, false), Color.Rgba(0, 0, 0, 0));
            for (float x = 0; x <= size.dx; x++)
            {
                shapes.Add(new Polyline(paint, Util.NewList(new Point<Internal>(x, 0), new Point<Internal>(x, size.dy))));
            }
            for (float y = 0; y <= size.dy; y++)
            {
                shapes.Add(new Polyline(paint, Util.NewList(new Point<Internal>(0, y), new Point<Internal>(size.dx, y))));
            }
            return new ShapeGroup(shapes.ToArray());
        }

        partial void Redisplay();
        partial void SetUndoAbility(bool b);
        partial void SetRedoAbility(bool b);
        partial void SetClearAbility(bool b);
        partial void ResetSecondCanvas();
        partial void SetActiveness();
        partial void SetIcons();
    }
}