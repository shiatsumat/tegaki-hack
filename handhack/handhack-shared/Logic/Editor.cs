using System;
using System.Collections.Generic;
using System.Xml.Linq;
using static handhack.UtilStatic;

namespace handhack
{
    public enum EShapeCreator { Freehand, Line, Oval, Rectangle }

    public enum Touchevent { Down, Move, Up }

    public partial class Editor
    {
        public DPoint<Internal> size;
        public DPoint<External> realsize;
        bool _strict;
        public bool strict { get { return _strict; } set { _strict = value; if (shapeCreator != null) shapeCreator.strict = value; } }
        Transform<Internal, External> transform;
        List<IShape> drawnShapes, undrawnShapes, redoShapes;
        public Paint paint, gridpaint;
        AShapeCreator shapeCreator;
        EShapeCreator eShapeCreator;
        IShape grid;

        public Action Redisplay;
        public Action<bool> SetUndoAbility, SetRedoAbility;

        public Editor(DPoint<Internal> size, Action update, Action<bool> setUndoAbility, Action<bool> setRedoAbility)
        {
            this.size = size;
            drawnShapes = new List<IShape>();
            undrawnShapes = new List<IShape>();
            redoShapes = new List<IShape>();
            paint = new Paint(new Color(0xadff2fff), new SizeEither(0.5f, true), default(Color), Linecap.Round, Linejoin.Round);
            gridpaint = new Paint(new Color(0xf5f5f5ff), new SizeEither(1, false), new Color(0, 0, 0, 0));
            strict = false;

            ChangeShapeCreator(EShapeCreator.Freehand);

            this.Redisplay = update;
            this.SetUndoAbility = setUndoAbility;
            this.SetRedoAbility = setRedoAbility;
            setUndoAbility(false);
            setRedoAbility(false);
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

        public void ChangeShapeCreator(EShapeCreator eShapeCreator)
        {
            shapeCreator?.Bye();
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
                default:
                    throw new InvalidOperationException("Invalid EShapeCreator for ChangeShapeCreator");
            }
            shapeCreator.paint = paint;
            shapeCreator.strict = strict;
            shapeCreator.Edited += () =>
            {
                redoShapes.Clear();
                SetRedoAbility(false);
                Redisplay();
            };
            shapeCreator.Finished += (shape) =>
            {
                if (shape != null) undrawnShapes.Add(shape);
                Redisplay();
            };
        }
        public void ResetShapeCreator()
        {
            ChangeShapeCreator(eShapeCreator);
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
            for (float x = 0; x <= size.dx; x++)
            {
                shapes.Add(new Polyline(gridpaint, newList(new Point<Internal>(x, 0), new Point<Internal>(x, size.dy))));
            }
            for (float y = 0; y <= size.dy; y++)
            {
                shapes.Add(new Polyline(gridpaint, newList(new Point<Internal>(0, y), new Point<Internal>(size.dx, y))));
            }
            grid = new ShapeGroup(shapes.ToArray());
        }
    }
}