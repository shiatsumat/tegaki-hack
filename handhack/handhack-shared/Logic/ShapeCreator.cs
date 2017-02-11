using System;
using System.Collections.Generic;
using Android.Graphics;
using static System.Math;
using static handhack.UtilStatic;

namespace handhack
{
    public abstract partial class AShapeCreator
    {
        public Paint paint;
        protected bool dragging;
        Point<Internal> prev;
        public Action Edited;
        public Action<IShape> Finished;

        public AShapeCreator()
        {
            dragging = false;
            Init();
        }
        public virtual void Touch(Touchevent touchevent, Point<Internal> p)
        {
            switch (touchevent)
            {
                case Touchevent.Down:
                    StartDrag(p);
                    break;
                case Touchevent.Move:
                    if (!dragging)
                    {
                        dragging = true;
                        prev = p;
                        StartDrag(p);
                    }
                    else if (prev.distance(p) > 1e-4) MoveDrag(p);
                    break;
                case Touchevent.Up:
                    if (dragging)
                    {
                        if (prev.distance(p) > 1e-4) MoveDrag(p);
                        EndDrag();
                    }
                    break;
            }
        }
        protected abstract void Init();
        protected abstract IShape Finish();
        protected abstract void StartDrag(Point<Internal> p);
        protected abstract void MoveDrag(Point<Internal> p);
        protected abstract void EndDrag();
        public abstract void Draw(Canvas canvas, Transform<Internal, External> transform);
        public void Cleanup()
        {
            Finished(Finish());
            Edited();
            dragging = false;
            Init();
        }
        public void Bye()
        {
            Finished(Finish());
            Edited();
        }

    }
    public partial class FreehandCreator : AShapeCreator
    {
        Polyline polyline;

        public FreehandCreator() { }
        protected override void Init()
        {
            polyline = null;
        }
        protected override IShape Finish()
        {
            return polyline;
        }
        protected override void StartDrag(Point<Internal> p)
        {
            polyline = new Polyline(paint, newList(p), true);
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            polyline.points.Add(p);
            Edited();
        }
        protected override void EndDrag()
        {
            Cleanup();
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            polyline?.Draw(canvas, transform);
        }
    }
    public partial class LineCreator : AShapeCreator
    {
        Polyline polyline;

        public LineCreator() { }
        protected override void Init()
        {
            polyline = null;
        }
        protected override IShape Finish()
        {
            return polyline;
        }
        protected override void StartDrag(Point<Internal> p)
        {
            polyline = new Polyline(paint, newList(p, default(Point<Internal>)));
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            polyline.points[1] = p;
            Edited();
        }
        protected override void EndDrag()
        {
            Cleanup();
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            polyline?.Draw(canvas, transform);
        }
    }
    public partial class CircleCreator : AShapeCreator
    {
        Oval oval;

        public CircleCreator() { }
        protected override void Init()
        {
            oval = null;
        }
        protected override IShape Finish()
        {
            return oval;
        }
        protected override void StartDrag(Point<Internal> p)
        {
            oval = new Oval(paint, p, default(DPoint<Internal>));
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            var r = oval.center.distance(p);
            oval.radii = new DPoint<Internal>(r, r);
            Edited();
        }
        protected override void EndDrag()
        {
            Cleanup();
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            oval?.Draw(canvas, transform);
        }
    }
    public partial class OvalCreator : AShapeCreator
    {
        Oval oval;

        public OvalCreator() { }
        protected override void Init()
        {
            oval = null;
        }
        protected override IShape Finish()
        {
            return oval;
        }
        protected override void StartDrag(Point<Internal> p)
        {
            oval = new Oval(paint, p, default(DPoint<Internal>));
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            var v = p - oval.center;
            oval.radii = v * (float)Sqrt(2);
            Edited();
        }
        protected override void EndDrag()
        {
            Cleanup();
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            oval?.Draw(canvas, transform);
        }
    }
}