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
        public bool strict;
        public Action Edited;
        public Action<IShape> Finished;
        protected bool dragging;
        Point<Internal> prev;

        public AShapeCreator()
        {
            dragging = false;
            strict = false;
            Init();
        }
        public virtual void Touch(Touchevent touchevent, Point<Internal> p)
        {
            switch (touchevent)
            {
                case Touchevent.Down:
                    if (dragging) { EndDrag(); }
                    dragging = true;
                    prev = p;
                    StartDrag(p);
                    break;
                case Touchevent.Move:
                    if (!dragging)
                    {
                        dragging = true;
                        prev = p;
                        StartDrag(p);
                    }
                    else if (prev.distance(p) > EPS) MoveDrag(p);
                    break;
                case Touchevent.Up:
                    if (dragging)
                    {
                        if (prev.distance(p) > EPS) MoveDrag(p);
                        dragging = false;
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
            if (!strict) polyline.points[1] = p;
            else
            {
                var from = polyline.points[0];
                var v = p - from;
                var unit = 15.0f;
                if (v.norm < EPS) polyline.points[1] = from;
                else
                {
                    var polar = Complex.Polar((float)Round(v.arg / unit) * unit, 1);
                    var polar2 = polar * ((Abs(v.dx) > Abs(v.dy)) ? v.dx / polar.re : v.dy / polar.im);
                    polyline.points[1] = from + new DPoint<Internal>(polar2);
                }
            }
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
    public partial class OvalCreator : AShapeCreator
    {
        Oval oval;

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
            if (strict)
            {
                var r = oval.center.distance(p);
                oval.radii = new DPoint<Internal>(r, r);
            }
            else
            {
                var v = p - oval.center;
                oval.radii = v * (float)Sqrt(2);
            }
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
    public partial class RectangleCreator : AShapeCreator
    {
        Polyline polyline;
        Point<Internal> from, to;

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
            from = to = p;
            polyline = new Polyline(paint, newList(p, p, p, p), false, true);
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            if (!strict) to = p;
            else
            {
                var v = p - from;
                float l = Max(Abs(v.dx), Abs(v.dy));
                to = from + new DPoint<Internal>(v.dx.ToAbs(l), v.dy.ToAbs(l));
            }
            polyline.points[1] = new Point<Internal>(from.x, to.y);
            polyline.points[2] = to;
            polyline.points[3] = new Point<Internal>(to.x, from.y);
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
    public partial class RegularPolygonCreator : AShapeCreator
    {
        Polyline polyline;
        Point<Internal> from, to;

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
            from = to = p;
            polyline = new Polyline(paint, newList(p, p, p, p), false, true);
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            if (!strict) to = p;
            else
            {
                var v = p - from;
                float l = Max(Abs(v.dx), Abs(v.dy));
                to = from + new DPoint<Internal>(v.dx.ToAbs(l), v.dy.ToAbs(l));
            }
            polyline.points[1] = new Point<Internal>(from.x, to.y);
            polyline.points[2] = to;
            polyline.points[3] = new Point<Internal>(to.x, from.y);
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
}