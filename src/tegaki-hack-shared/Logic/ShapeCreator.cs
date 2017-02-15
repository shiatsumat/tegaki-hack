using System;
using System.Collections.Generic;
using Android.Graphics;

namespace tegaki_hack
{

    public static partial class ShapeCreator
    {
        static Exception InvalidShapeCreator()
        {
            return new InvalidOperationException("Invalid EShapeCreator for ChangeShapeCreator");
        }
        public static AShapeCreator GetShapeCreator(EShapeCreator eShapeCreator)
        {
            switch (eShapeCreator)
            {
                case EShapeCreator.None:
                    return null;
                case EShapeCreator.Freehand:
                    return new FreehandCreator();
                case EShapeCreator.Line:
                    return new LineCreator();
                case EShapeCreator.Polyline:
                    return new PolylineCreator();
                case EShapeCreator.Arc:
                    return new ArcCreator();
                case EShapeCreator.Oval:
                    return new OvalCreator();
                case EShapeCreator.Rectangle:
                    return new RectangleCreator();
                case EShapeCreator.RegularPolygon:
                    return new RegularPolygonCreator();
                case EShapeCreator.Polygon:
                    return new PolygonCreator();
                case EShapeCreator.Text:
                    return new TextCreator();
                case EShapeCreator.FancyText:
                    return new FancyTextCreator();
                default:
                    throw InvalidShapeCreator();
            }
        }
    }

    public enum EShapeCreator { None, Freehand, Line, Polyline, Arc, Oval, Rectangle, RegularPolygon, Polygon, Text, FancyText }

    public partial class ShapeCreatorSettings
    {
        public bool DoesAdjust;
        public Adjustment Adjustment;
        public bool Regulation;
        public Paint Paint;
        public Paint GuidePaint;
        public int NRegularPolygon;
        public Action Edited;
        public Action<IShape> Finished;

        public ShapeCreatorSettings(Action edited, Action<IShape> finished)
        {
            Paint = new Paint(Color.Rgba(0xadff2fff), new SizeEither(0.5f, true), lineCap: LineCap.Round, lineJoin: LineJoin.Round);
            DoesAdjust = false;
            Adjustment = new Adjustment();
            NRegularPolygon = 3;
            Edited = edited;
            Finished = finished;
        }
    }

    public enum CoordinateAdjustment { None, Integer, Existing }
    public partial class Adjustment
    {
        public CoordinateAdjustment XAdjustment, YAdjustment;
        public bool DoesAdjustAngle;
        public int RightAngleDivision;
        public bool DoesAdjustLength;

        public Adjustment()
        {
            XAdjustment = CoordinateAdjustment.Integer;
            YAdjustment = CoordinateAdjustment.Integer;
            DoesAdjustAngle = false;
            RightAngleDivision = 6;
            DoesAdjustLength = false;
        }
        public Adjustment(CoordinateAdjustment xAdjustment, CoordinateAdjustment yAdjustment,
            bool doesAdjustAngle, int rightAngleDivision, bool doesAdjustLength)
        {
            XAdjustment = xAdjustment; YAdjustment = yAdjustment;
            DoesAdjustAngle = doesAdjustAngle; RightAngleDivision = rightAngleDivision; DoesAdjustLength = doesAdjustLength;
        }
        public Adjustment(Adjustment adjustment)
            : this(adjustment.XAdjustment, adjustment.YAdjustment,
                 adjustment.DoesAdjustAngle, adjustment.RightAngleDivision, adjustment.DoesAdjustLength)
        { }

        public bool Equals(Adjustment adjustment)
        {
            return
                XAdjustment == adjustment.XAdjustment &&
                YAdjustment == adjustment.YAdjustment &&
                DoesAdjustAngle == adjustment.DoesAdjustAngle &&
                RightAngleDivision == adjustment.RightAngleDivision &&
                DoesAdjustLength == adjustment.DoesAdjustLength;
        }

        public Point<Internal> Adjust(Point<Internal> p)
        {
            if (XAdjustment == CoordinateAdjustment.Integer)
            {
                p.x = (float)Math.Round(p.x);
            }
            if (YAdjustment == CoordinateAdjustment.Integer)
            {
                p.y = (float)Math.Round(p.y);
            }
            return p;
        }
        public Point<Internal> Adjust(Point<Internal> p, Point<Internal> prev)
        {
            p = Adjust(p);
            if ((XAdjustment == CoordinateAdjustment.None || YAdjustment == CoordinateAdjustment.None)
                && DoesAdjustAngle)
            {
                return Geometry.AdjustAngle(prev, p, RightAngleDivision);
            }
            else return p;
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
                    else if (prev.distance(p) > Util.EPS) MoveDrag(p);
                    break;
                case Touchevent.Up:
                    if (dragging)
                    {
                        if (prev.distance(p) > Util.EPS) MoveDrag(p);
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
            settings.Finished?.Invoke(Finish());
            dragging = false;
        }

        protected void Edited()
        {
            settings.Edited?.Invoke();
        }

        protected Point<Internal> Adjust(Point<Internal> p)
        {
            if (!settings.DoesAdjust) return p;
            else return settings.Adjustment.Adjust(p);
        }
        protected Point<Internal> Adjust(Point<Internal> p, Point<Internal> prev)
        {
            if (!settings.DoesAdjust) return p;
            else return settings.Adjustment.Adjust(p, prev);
        }
    }

    public partial class FreehandCreator : AShapeCreator
    {
        Polyline polyline;

        protected override IShape Finish()
        {
            if (polyline == null) return null;
            var oldpolyline = polyline;
            polyline = null;
            return oldpolyline.points.Count >= 2 ? oldpolyline : null;
        }
        protected override void StartDrag(Point<Internal> p)
        {
            polyline = new Polyline(settings.Paint, Util.NewList(p), false, true);
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

    public abstract partial class TwoPointCreator : AShapeCreator
    {
        protected Point<Internal>? from, to;

        protected sealed override void StartDrag(Point<Internal> p)
        {
            from = Adjust(p);
            Set();
        }
        protected sealed override void MoveDrag(Point<Internal> p)
        {
            to = Adjust(p);
            Set();
            Edited();
        }
        protected sealed override void EndDrag()
        {
            Cleanup();
            from = to = null;
        }

        protected abstract void Set();
    }

    public abstract partial class TwoPointLineCreator : AShapeCreator
    {
        protected Point<Internal>? from, to;

        protected sealed override void StartDrag(Point<Internal> p)
        {
            from = Adjust(p);
            Set();
        }
        protected sealed override void MoveDrag(Point<Internal> p)
        {
            to = Adjust(p, from.Value);
            Set();
            Edited();
        }
        protected sealed override void EndDrag()
        {
            Cleanup();
            from = to = null;
        }

        protected abstract void Set();
    }

    public partial class LineCreator : TwoPointLineCreator
    {
        Polyline polyline;

        protected override IShape Finish()
        {
            return Util.Nulling(ref polyline);
        }
        protected override void Set()
        {
            if (from != null && to != null)
            {
                polyline = new Polyline(settings.Paint, Util.NewList(from.Value, to.Value));
            }
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            polyline?.Draw(canvas, transform);
        }
    }

    public partial class PolylineCreator : AShapeCreator
    {
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            // TODO
        }

        protected override void EndDrag()
        {
            // TODO
        }

        protected override IShape Finish()
        {
            // TODO
            return null;
        }

        protected override void MoveDrag(Point<Internal> p)
        {
            // TODO
        }

        protected override void StartDrag(Point<Internal> p)
        {
            // TODO
        }
    }

    public partial class ArcCreator : AShapeCreator
    {
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            // TODO
        }

        protected override void EndDrag()
        {
            // TODO
        }

        protected override IShape Finish()
        {
            // TODO
            return null;
        }

        protected override void MoveDrag(Point<Internal> p)
        {
            // TODO
        }

        protected override void StartDrag(Point<Internal> p)
        {
            // TODO
        }
    }

    public partial class OvalCreator : TwoPointCreator
    {
        Oval oval;

        protected override IShape Finish()
        {
            return Util.Nulling(ref oval);
        }
        protected override void Set()
        {
            if (from != null && to != null)
            {
                DPoint<Internal> radii;
                if (settings.Regulation)
                {
                    var r = from.Value.distance(to.Value);
                    radii = new DPoint<Internal>(r, r);
                }
                else
                {
                    var v = to.Value - from.Value;
                    radii = v * (float)Math.Sqrt(2);
                }
                oval = new Oval(settings.Paint, from.Value, radii);
            }
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            oval?.Draw(canvas, transform);
        }
    }

    public partial class RectangleCreator : TwoPointCreator
    {
        Polyline polyline;

        protected override IShape Finish()
        {
            return Util.Nulling(ref polyline);
        }
        protected override void Set()
        {
            if (from != null && to != null)
            {
                var upperleft = from.Value;
                var lowerright = !settings.Regulation ? to.Value : Geometry.AdjustSquare(upperleft, to.Value);
                var lowerleft = new Point<Internal>(upperleft.x, lowerright.y);
                var upperright = new Point<Internal>(lowerright.x, upperleft.y);
                polyline = new Polyline(settings.Paint, Util.NewList(upperleft, lowerleft, lowerright, upperright), true);
            }
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            polyline?.Draw(canvas, transform);
        }
    }

    public partial class RegularPolygonCreator : TwoPointLineCreator
    {
        Polyline polyline;

        protected override IShape Finish()
        {
            return Util.Nulling(ref polyline);
        }
        protected override void Set()
        {
            if (from != null && to != null)
            {
                polyline = new Polyline(settings.Paint, Util.NewList<Point<Internal>>(), true);
                polyline.points.Add(from.Value);
                polyline.points.Add(to.Value);
                for (int i = 2; i < settings.NRegularPolygon; i++)
                {
                    var prev = polyline.points[i - 1];
                    var prev2 = polyline.points[i - 2];
                    var v = prev - prev2;
                    polyline.points.Add(prev + new DPoint<Internal>(Complex.Polar(v.arg - 360.0f / settings.NRegularPolygon, v.norm)));
                }
            }
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            polyline?.Draw(canvas, transform);
        }
    }

    public partial class PolygonCreator : AShapeCreator
    {
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            // TODO
        }

        protected override void EndDrag()
        {
            // TODO
        }

        protected override IShape Finish()
        {
            // TODO
            return null;
        }

        protected override void MoveDrag(Point<Internal> p)
        {
            // TODO
        }

        protected override void StartDrag(Point<Internal> p)
        {
            // TODO
        }
    }

    public partial class TextCreator : AShapeCreator
    {
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            // TODO
        }

        protected override void EndDrag()
        {
            // TODO
        }

        protected override IShape Finish()
        {
            // TODO
            return null;
        }

        protected override void MoveDrag(Point<Internal> p)
        {
            // TODO
        }

        protected override void StartDrag(Point<Internal> p)
        {
            // TODO
        }
    }

    public partial class FancyTextCreator : AShapeCreator
    {
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            // TODO
        }

        protected override void EndDrag()
        {
            // TODO
        }

        protected override IShape Finish()
        {
            // TODO
            return null;
        }

        protected override void MoveDrag(Point<Internal> p)
        {
            // TODO
        }

        protected override void StartDrag(Point<Internal> p)
        {
            // TODO
        }
    }

}