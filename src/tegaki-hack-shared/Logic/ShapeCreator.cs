using System;
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
                    return new FreehandCreator(false);
                case EShapeCreator.ClosedFreehand:
                    return new FreehandCreator(true);
                case EShapeCreator.Line:
                    return new LineCreator();
                case EShapeCreator.Polyline:
                    return new PolylineCreator();
                case EShapeCreator.Arc:
                    return new ArcCreator();
                case EShapeCreator.Circle:
                    return new CircleCreator();
                case EShapeCreator.Ellipse:
                    return new EllipseCreator();
                case EShapeCreator.Square:
                    return new RectangleCreator(true);
                case EShapeCreator.Rectangle:
                    return new RectangleCreator(false);
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

    public enum EShapeCreator
    {
        None,
        Freehand, ClosedFreehand,
        Line, Polyline, Arc,
        Circle, Ellipse, Rectangle, Square, RegularPolygon, Polygon,
        Text, FancyText
    }

    public partial class ShapeCreatorSettings
    {
        public bool DoesAdjust;
        public Adjustment Adjustment;
        public Drawing Drawing;
        public Drawing GuideDrawing;
        public int NRegularPolygon;
        public Action Edited;
        public Action<IShape> Finished;

        public ShapeCreatorSettings(Action edited, Action<IShape> finished)
        {
            Drawing = new Drawing(Color.ByRgba(0x91ff4cff), Color.ByRgba(0x66f0ff40), new SizeEither(0.5f, true),
                LineCap.Round, LineJoin.Round);
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
        public bool AngleAdjustmentAvailable =>
                XAdjustment == CoordinateAdjustment.None ||
                YAdjustment == CoordinateAdjustment.None;
        public bool LengthAdjustmentAvailable => Util.ZeroOrOne(
                XAdjustment != CoordinateAdjustment.None,
                YAdjustment != CoordinateAdjustment.None,
                DoesAdjustAngle);

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
                p.X = (float)Math.Round(p.X);
            }
            if (YAdjustment == CoordinateAdjustment.Integer)
            {
                p.Y = (float)Math.Round(p.Y);
            }
            return p;
        }
        public Point<Internal> Adjust(Point<Internal> p, Point<Internal> prev)
        {
            p = Adjust(p);
            if (p.DistanceTo(prev) < Util.EPS) return p;
            if (XAdjustment == CoordinateAdjustment.None || YAdjustment == CoordinateAdjustment.None)
            {
                if (DoesAdjustAngle) return AdjustAngle(p, prev);
                else if (DoesAdjustLength) return AdjustLength(p, prev);
            }
            return p;
        }
        Point<Internal> AdjustAngle(Point<Internal> p, Point<Internal> prev)
        {
            var v = p - prev;
            var angleUnit = 90.0f / RightAngleDivision;
            var polar = Complex.Polar((float)Math.Round(v.Arg / angleUnit) * angleUnit);
            float ratio = 0;
            if (XAdjustment != CoordinateAdjustment.None)
            {
                if (polar.Re < Util.EPS) return p;
                ratio = v.Dx / polar.Re;
            }
            else if (YAdjustment != CoordinateAdjustment.None)
            {
                if (polar.Im < Util.EPS) return p;
                ratio = v.Dy / polar.Im;
            }
            else if (!DoesAdjustLength) ratio = Math.Abs(v.Dx) > Math.Abs(v.Dy) ? v.Dx / polar.Re : v.Dy / polar.Im;
            else ratio = (float)Math.Round(v.Norm) / polar.Norm;
            return prev + new DPoint<Internal>(polar * ratio);
        }
        Point<Internal> AdjustLength(Point<Internal> p, Point<Internal> prev)
        {
            var v = p - prev;
            var l = (float)Math.Round(v.Norm);
            if (XAdjustment != CoordinateAdjustment.None)
            {
                if (Math.Abs(v.Dx) < l) l++;
                v.Dy = v.Dy.AdjustAbs((float)Math.Sqrt(l * l - v.Dx * v.Dx));
                return prev + v;
            }
            else if (YAdjustment != CoordinateAdjustment.None)
            {
                if (Math.Abs(v.Dy) < l) l++;
                v.Dx = v.Dx.AdjustAbs((float)Math.Sqrt(l * l - v.Dy * v.Dy));
                return prev + v;
            }
            else
            {
                return prev + v * l / v.Norm;
            }
        }
    }

    public abstract partial class AShapeCreator
    {
        public ShapeCreatorSettings Settings;
        protected bool dragging;
        Point<Internal> prev;

        public AShapeCreator()
        {
            dragging = false;
        }
        public virtual void Touch(TouchEvent touchEvent, Point<Internal> p)
        {
            switch (touchEvent)
            {
                case TouchEvent.Down:
                    if (dragging) { EndDrag(); }
                    dragging = true;
                    prev = p;
                    StartDrag(p);
                    break;
                case TouchEvent.Move:
                    if (!dragging)
                    {
                        dragging = true;
                        prev = p;
                        StartDrag(p);
                    }
                    else if (prev.DistanceTo(p) > Util.EPS) MoveDrag(p);
                    break;
                case TouchEvent.Up:
                    if (dragging)
                    {
                        if (prev.DistanceTo(p) > Util.EPS) MoveDrag(p);
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
            Settings.Finished?.Invoke(Finish());
            dragging = false;
        }

        protected void Edited()
        {
            Settings.Edited?.Invoke();
        }

        protected Point<Internal> Adjust(Point<Internal> p)
        {
            if (!Settings.DoesAdjust) return p;
            else return Settings.Adjustment.Adjust(p);
        }
        protected Point<Internal> Adjust(Point<Internal> p, Point<Internal> prev)
        {
            if (!Settings.DoesAdjust) return p;
            else return Settings.Adjustment.Adjust(p, prev);
        }
    }

    public partial class FreehandCreator : AShapeCreator
    {
        bool closed;
        Polyline polyline;

        public FreehandCreator(bool closed)
        {
            this.closed = closed;
        }
        protected override IShape Finish()
        {
            if (polyline == null) return null;
            var res = polyline;
            polyline = null;
            res.Closed = closed;
            return res.Points.Count >= 2 ? res : null;
        }
        protected override void StartDrag(Point<Internal> p)
        {
            polyline = new Polyline(Settings.Drawing, Util.NewList(p), false, true);
        }
        protected override void MoveDrag(Point<Internal> p)
        {
            polyline.Points.Add(p);
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

    public partial class LineCreator : TwoPointCreator
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
                polyline = new Polyline(Settings.Drawing, Util.NewList(from.Value, to.Value));
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

    public partial class CircleCreator : TwoPointCreator
    {
        Circle circle;

        protected override IShape Finish()
        {
            return Util.Nulling(ref circle);
        }
        protected override void Set()
        {
            if (from != null && to != null)
            {
                circle = new Circle(Settings.Drawing, from.Value, new SizeEither(from.Value.DistanceTo(to.Value), true));
            }
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            circle?.Draw(canvas, transform);
        }
    }

    public partial class EllipseCreator : AShapeCreator
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

    public partial class RectangleCreator : TwoPointCreator
    {
        bool square;
        Polyline polyline;

        public RectangleCreator(bool square)
        {
            this.square = square;
        }
        protected override IShape Finish()
        {
            return Util.Nulling(ref polyline);
        }
        protected override void Set()
        {
            if (from != null && to != null)
            {
                var upperleft = from.Value;
                Point<Internal> lowerright;
                if (!square) lowerright = to.Value;
                else
                {
                    var v = to.Value - from.Value;
                    float l = Math.Max(Math.Abs(v.Dx), Math.Abs(v.Dy));
                    lowerright = from.Value + new DPoint<Internal>(v.Dx.AdjustAbs(l), v.Dy.AdjustAbs(l));
                }
                var lowerleft = new Point<Internal>(upperleft.X, lowerright.Y);
                var upperright = new Point<Internal>(lowerright.X, upperleft.Y);
                polyline = new Polyline(Settings.Drawing, Util.NewList(upperleft, lowerleft, lowerright, upperright), true);
            }
        }
        public override void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            polyline?.Draw(canvas, transform);
        }
    }

    public partial class RegularPolygonCreator : TwoPointCreator
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
                polyline = new Polyline(Settings.Drawing, Util.NewList<Point<Internal>>(), true);
                polyline.Points.Add(from.Value);
                polyline.Points.Add(to.Value);
                for (int i = 2; i < Settings.NRegularPolygon; i++)
                {
                    var prev = polyline.Points[i - 1];
                    var prev2 = polyline.Points[i - 2];
                    var v = prev - prev2;
                    polyline.Points.Add(prev + new DPoint<Internal>(Complex.Polar(v.Arg - 360.0f / Settings.NRegularPolygon, v.Norm)));
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