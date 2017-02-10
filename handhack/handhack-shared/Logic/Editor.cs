using System;
using System.Collections.Generic;
using static handhack.UtilStatic;

namespace handhack
{
    public enum ShapeCreator { Freehand, Line, Circle }

    public enum Touchevent { Down, Move, Up }

    public partial class Editor : IDrawable
    {
        public DPoint<Internal> size;
        public float width { get { return size.dx; } set { size.dx = value; } }
        public Size<Internal> Width { get { return new Size<Internal>(width); } set { width = value.value; } }
        public float height { get { return size.dy; } set { size.dy = value; } }
        public Size<Internal> Height { get { return new Size<Internal>(height); } set { height = value.value; } }
        List<IShape> drawnShapes, undrawnShapes, redoShapes;
        public Paint paint, gridpaint;
        AShapeCreator shapeCreator;
        ShapeCreator shapeCreatorId;
        IShape grid;

        public Action update;
        public BoolAction setUndoAbility, setRedoAbility;

        public Editor(DPoint<Internal> size, Action update, BoolAction setUndoAbility, BoolAction setRedoAbility)
        {
            this.size = size;
            drawnShapes = new List<IShape>();
            undrawnShapes = new List<IShape>();
            redoShapes = new List<IShape>();
            paint = new Paint(new Color(0xadff2fff), new SizeEither(0.5f, true), default(Color), Linecap.Round, Linejoin.Round);
            gridpaint = new Paint(new Color(0xf5f5f5ff), new SizeEither(1, false), new Color(0, 0, 0, 0));

            ChangeShapeCreator(ShapeCreator.Freehand);

            this.update = update;
            this.setUndoAbility = setUndoAbility;
            this.setRedoAbility = setRedoAbility;
            setUndoAbility(false);
            setRedoAbility(false);
        }

        public void Undo()
        {
            if (drawnShapes.Count > 0 && undrawnShapes.Count == 0)
            {
                redoShapes.Add(drawnShapes.Pop());
                setRedoAbility(true);
                setUndoAbility(drawnShapes.Count > 0);
                undrawnShapes = drawnShapes;
                drawnShapes = new List<IShape>();
                ResetSecondCanvas();
                Update();
            }
            else throw new InvalidOperationException("Editor Not Undoable!");
        }
        public void Redo()
        {
            if (redoShapes.Count > 0)
            {
                undrawnShapes.Add(redoShapes.Pop());
                setRedoAbility(redoShapes.Count > 0);
                Update();
            }
            else throw new InvalidOperationException("Editor Not Redoable!");
        }
        public void Touch(Touchevent touchevent, Point<Internal> p)
        {
            if (shapeCreator != null)
            {
                shapeCreator.Touch(touchevent, p);
            }
        }
        public void Update()
        {
            update();
        }
        void SetGrid()
        {
            var shapes = new List<IShape>();
            for (float x = 0; x <= width; x++)
            {
                shapes.Add(new Polyline(gridpaint, newList(new Point<Internal>(x, 0), new Point<Internal>(x, height))));
            }
            for (float y = 0; y <= height; y++)
            {
                shapes.Add(new Polyline(gridpaint, newList(new Point<Internal>(0, y), new Point<Internal>(width, y))));
            }
            grid = new ShapeGroup(shapes.ToArray());
        }

        public void ChangeShapeCreator(ShapeCreator shapeCreatorId)
        {
            if (shapeCreator != null) shapeCreator.Bye();
            this.shapeCreatorId = shapeCreatorId;
            switch (shapeCreatorId)
            {
                case ShapeCreator.Freehand:
                    shapeCreator = new FreehandCreator(paint);
                    break;
                case ShapeCreator.Line:
                    shapeCreator = new LineCreator(paint);
                    break;
                case ShapeCreator.Circle:
                    shapeCreator = new CircleCreator(paint);
                    break;
                default:
                    break;
            }
            shapeCreator.edited += () =>
            {
                redoShapes.Clear();
                setRedoAbility(false);
                Update();
            };
            shapeCreator.finish += () =>
            {
                if (shapeCreator.shape != null) undrawnShapes.Add(shapeCreator.shape);
                Update();
            };
        }
        public void ResetShapeCreator()
        {
            ChangeShapeCreator(shapeCreatorId);
        }
    }
}