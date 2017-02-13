using System;
using System.Collections.Generic;
using System.Xml.Linq;
using static tegaki_hack.UtilStatic;

namespace tegaki_hack
{
    public partial class Editor
    {
        public DPoint<Internal> size;
        public DPoint<External> realsize;
        Transform<Internal, External> transform;
        List<IShape> drawnShapes, undrawnShapes, redoShapes;
        EShapeCreator eShapeCreator;
        public ShapeCreatorSettings settings;
        AShapeCreator shapeCreator;
        IShape grid;

        public Action Redisplay;
        public Action<bool> SetUndoAbility, SetRedoAbility;

        public Editor(DPoint<Internal> size, Action Redisplay, Action<bool> SetUndoAbility, Action<bool> SetRedoAbility)
        {
            this.size = size;
            drawnShapes = new List<IShape>();
            undrawnShapes = new List<IShape>();
            redoShapes = new List<IShape>();

            settings = new ShapeCreatorSettings(() =>
            {
                redoShapes.Clear();
                this.SetRedoAbility(false);
                this.Redisplay();
            }, (shape) =>
            {
                if (shape != null) undrawnShapes.Add(shape);
                this.Redisplay();
            });
            ChangeShapeCreator(EShapeCreator.Freehand);

            this.Redisplay = Redisplay;
            this.SetUndoAbility = SetUndoAbility;
            this.SetRedoAbility = SetRedoAbility;
            SetUndoAbility(false);
            SetRedoAbility(false);
        }

        public void DealWithLayoutChange(DPoint<External> realsize)
        {
            this.realsize = realsize;
            transform.scale = realsize.dx / size.dx;

            ResetSecondCanvas();
            Redisplay();
        }

        public void Undo()
        {
            if (drawnShapes.Count > 0 && undrawnShapes.Count == 0)
            {
                redoShapes.Add(drawnShapes.Pop());
                SetRedoAbility(true);
                ResetSecondCanvas();
                Redisplay();
            }
            else throw new InvalidOperationException("Editor Not Undoable!");
        }
        public void Redo()
        {
            if (redoShapes.Count > 0)
            {
                undrawnShapes.Add(redoShapes.Pop());
                SetRedoAbility(redoShapes.Count > 0);
                Redisplay();
            }
            else throw new InvalidOperationException("Editor Not Redoable!");
        }
        public void Touch(Touchevent touchevent, Point<External> p)
        {
            shapeCreator?.Touch(touchevent, p.Untransform(transform));
        }

        void ChangeShapeCreator(EShapeCreator eShapeCreator)
        {
            shapeCreator?.Cleanup();
            this.eShapeCreator = eShapeCreator;
            switch (eShapeCreator)
            {
                case EShapeCreator.Freehand:
                    shapeCreator = new FreehandCreator();
                    break;
                case EShapeCreator.Line:
                    shapeCreator = new LineCreator();
                    break;
                case EShapeCreator.Oval:
                    shapeCreator = new OvalCreator();
                    break;
                case EShapeCreator.Rectangle:
                    shapeCreator = new RectangleCreator();
                    break;
                case EShapeCreator.RegularPolygon:
                    shapeCreator = new RegularPolygonCreator();
                    break;
                default:
                    throw new InvalidOperationException("Invalid EShapeCreator for ChangeShapeCreator");
            }
            shapeCreator.settings = settings;
        }
        public void SetShapeCreator(EShapeCreator eShapeCreator)
        {
            if (this.eShapeCreator != eShapeCreator) ChangeShapeCreator(eShapeCreator);
        }
        public void ResetShapeCreator()
        {
            ChangeShapeCreator(eShapeCreator);
        }
        public void ResetShapeCreator(EShapeCreator eShapeCreator)
        {
            if (this.eShapeCreator == eShapeCreator) ChangeShapeCreator(eShapeCreator);
        }

        public XDocument GetSvg()
        {
            var shapes = new List<IShape>();
            shapes.AddRange(drawnShapes);
            shapes.AddRange(undrawnShapes);
            var svg = new XElement(svgName("svg"),
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
            SetUndoAbility(drawnShapes.Count > 0);
        }
        void MoveDrawnShapesToUndrawn()
        {
            if (drawnShapes.Count > 0)
            {
                drawnShapes.AddRange(undrawnShapes);
                undrawnShapes = drawnShapes;
                drawnShapes = new List<IShape>();
            }
        }
        void SetGrid()
        {
            var shapes = new List<IShape>();
            var paint = new Paint(new Color(0xd3d3d3ff), new SizeEither(1, false), new Color(0, 0, 0, 0));
            for (float x = 0; x <= size.dx; x++)
            {
                shapes.Add(new Polyline(paint, newList(new Point<Internal>(x, 0), new Point<Internal>(x, size.dy))));
            }
            for (float y = 0; y <= size.dy; y++)
            {
                shapes.Add(new Polyline(paint, newList(new Point<Internal>(0, y), new Point<Internal>(size.dx, y))));
            }
            grid = new ShapeGroup(shapes.ToArray());
        }
    }
}