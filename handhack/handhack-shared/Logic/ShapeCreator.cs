using System;
using System.Collections.Generic;

namespace handhack
{
    public abstract partial class AShapeCreator
    {
        public Paint paint;
        public IShape shape;
        public Action edited, finished;

        public abstract void Touch(Touchevent touchevent, Point<Internal> p);
    }
    public partial class FreehandCreator : AShapeCreator
    {
        public List<Point<Internal>> points;
        bool working;

        public FreehandCreator(Paint paint)
        {
            this.paint = paint;
            Init();
        }

        void Init()
        {
            points = new List<Point<Internal>>();
            this.shape = new Polyline(paint, points, true);
            working = false;
        }

        override public void Touch(Touchevent touchevent, Point<Internal> p)
        {
            if (points.Count > 0 && points[points.Count - 1].distance(p) < 1e-6) return;
            switch (touchevent)
            {
                case Touchevent.Down:
                    working = true;
                    points.Add(p);
                    edited();
                    break;
                case Touchevent.Move:
                    if (working)
                    {
                        points.Add(p);
                        edited();
                    }
                    break;
                case Touchevent.Up:
                    working = false;
                    points.Add(p);
                    edited();
                    Finished();
                    break;
            }
        }

        public void Finished()
        {
            finished();
            Init();
        }

        public void Bye()
        {
            finished();
        }
    }
}