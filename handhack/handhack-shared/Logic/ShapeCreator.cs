using System;
using System.Collections.Generic;
using Android.Graphics;
using static System.Math;
using static handhack.GeometryStatic;
using static handhack.UtilStatic;

namespace handhack
{
    public enum EShapeCreator { Freehand, Line, Oval, Rectangle, RegularPolygon }

    public partial class ShapeCreatorSettings
    {
        public Paint paint;
        public bool adjustment;
        public int rightAngleDivision;
        public int nRegularPolygon;
        public Action Edited;
        public Action<IShape> Finished;

        public ShapeCreatorSettings(Action Edited, Action<IShape> Finished)
        {
            paint = new Paint(new Color(0xadff2fff), new SizeEither(0.5f, true), default(Color), Linecap.Round, Linejoin.Round);
            adjustment = false;
            rightAngleDivision = 6;
            nRegularPolygon = 3;
            this.Edited = Edited;
            this.Finished = Finished;
        }
    }

    public abstract partial class AShapeCreator
    {
        public ShapeCreatorSettings settings;
        protected bool dragging;
        Point<Internal> prev;

        public AShapeCreator()
        {
            dragging = false;
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
        protected abstract IShape Finish();
        protected abstract void StartDrag(Point<Internal> p);
        protected abstract void MoveDrag(Point<Internal> p);
        protected abstract void EndDrag();
        public abstract void Draw(Canvas canvas, Transform<Internal, External> transform);
        public void Cleanup()
        {
            settings.Finished(Finish());
            dragging = false;
        }
    }
    public partial class FreehandCreator : AShapeCreator
    {
        Polyline polyline;

        protected override IShape Finish()
        {
            return Nulling(ref polyline);
        }
        protected override void StartDrag(Point<Internal> p)
        {
            polyline = new Polyline(settings.paint, newList(p), false, true);
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            polyline.points.Add(p);
            settings.Edited();
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

        protected override IShape Finish()
        {
            return Nulling(ref polyline);
        }
        protected override void StartDrag(Point<Internal> p)
        {
            polyline = new Polyline(settings.paint, newList(p, p));
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            polyline.points[1] = !settings.adjustment ? p : AdjustAngle(polyline.points[0], p, settings.rightAngleDivision);
            settings.Edited();
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

        protected override IShape Finish()
        {
            return Nulling(ref oval);
        }
        protected override void StartDrag(Point<Internal> p)
        {
            oval = new Oval(settings.paint, p, default(DPoint<Internal>));
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            if (settings.adjustment)
            {
                var r = oval.center.distance(p);
                oval.radii = new DPoint<Internal>(r, r);
            }
            else
            {
                var v = p - oval.center;
                oval.radii = v * (float)Sqrt(2);
            }
            settings.Edited();
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

        protected override IShape Finish()
        {
            return Nulling(ref polyline);
        }
        protected override void StartDrag(Point<Internal> p)
        {
            polyline = new Polyline(settings.paint, newList(p, p, p, p), true);
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            var from = polyline.points[0];
            if (!settings.adjustment) polyline.points[2] = p;
            else polyline.points[2] = AdjustSquare(from, p);
            var to = polyline.points[2];
            polyline.points[1] = new Point<Internal>(from.x, to.y);
            polyline.points[3] = new Point<Internal>(to.x, from.y);
            settings.Edited();
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

        protected override IShape Finish()
        {
            return Nulling(ref polyline);
        }
        protected override void StartDrag(Point<Internal> p)
        {
            polyline = new Polyline(settings.paint, new List<Point<Internal>>(), true);
            for (int i = 0; i < settings.nRegularPolygon; i++) polyline.points.Add(p);
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            var from = polyline.points[0];
            if (!settings.adjustment) polyline.points[1] = p;
            else polyline.points[1] = AdjustAngle(from, p, settings.rightAngleDivision);
            for (int i = 2; i < settings.nRegularPolygon; i++)
            {
                var prev = polyline.points[i - 1];
                var prev2 = polyline.points[i - 2];
                var v = prev - prev2;
                polyline.points[i] = prev + new DPoint<Internal>(Complex.Polar(v.arg - 360.0f / settings.nRegularPolygon, v.norm));
            }
            settings.Edited();
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