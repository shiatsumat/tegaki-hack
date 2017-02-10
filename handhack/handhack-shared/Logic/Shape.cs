using System;
using System.Xml.Linq;
using static handhack.GeometryStatic;

namespace handhack
{
    public partial interface IShape
    {
        void AddSvg<X>(XElement element, Transform<Internal, X> transform) where X : External;
    }
    public static partial class ShapeStatic
    {
        public static XElement AddSvg<X>(this XElement element, IShape shape, Transform<Internal, X> transform) where X : External
        {
            shape.AddSvg(element, transform);
            return element;
        }
    }

    public partial class Polyline : IShape
    {
        public Paint paint;
        public Point<Internal>[] points;
        public Point<Internal> startPoint
        {
            get { return points[0]; }
            set { points[0] = value; }
        }
        public Point<Internal> endPoint
        {
            get { return points[points.Length - 1]; }
            set { points[points.Length - 1] = value; }
        }
        public bool bezier;
        public bool closed;

        public Polyline(Paint paint, Point<Internal>[] vertices, bool bezier, bool closed)
        {
            this.paint = paint; this.points = vertices; this.bezier = bezier; this.closed = closed;
        }

        public void AddSvg<X>(XElement element, Transform<Internal, X> transform) where X : External
        {
            if (points.Length >= 2)
            {
                var dString = "";
                dString += string.Format("M {0}", startPoint);
                for (int i = 1; closed ? i < points.Length : i < points.Length + 1; i++)
                {
                    if (!bezier)
                    {
                        dString += string.Format("L {0}", points.LoopGet(i).Transform(transform));
                    }
                    else
                    {
                        var p0 = closed || i >= 2 ?
                            points.LoopGet(i - 2) :
                            startPoint;
                        var p1 = points.LoopGet(i - 1);
                        var p2 = points.LoopGet(i);
                        var p3 = closed || i < points.Length - 1 ?
                            points.LoopGet(i + 1) :
                            endPoint;
                        var conT = InterpolateCon(p0, p1, p2, p3).Transform(transform);
                        var trolT = InterpolateTrol(p0, p1, p2, p3).Transform(transform);
                        var toT = p2.Transform(transform);
                        dString += string.Format("C {0} {1} {2}", conT, trolT, toT);
                    }
                }
                element.Add(new XElement("path",
                    new XAttribute("d", dString))
                    .AddSvg(paint, transform));
            }
        }
    }

    public partial class Oval : IShape
    {
        public Paint paint;
        public Point<Internal> center;
        DPoint<Internal> _radii;
        public virtual DPoint<Internal> radii
        {
            get { return _radii; }
            set
            {
                if (value._dx < 0 || value._dy < 0)
                {
                    throw new InvalidOperationException("negative radius for Oval");
                }
                _radii = value;
            }
        }

        public Oval(Paint paint, Point<Internal> center, DPoint<Internal> radii)
        {
            this.paint = paint; this.center = center; this.radii = radii;
        }

        public void AddSvg<X>(XElement element, Transform<Internal, X> transform) where X : External
        {
            element.Add(new XElement("ellipse")
                .AddSvg(paint, transform)
                .AddSvg(center, "cx", "cy", transform)
                .AddSvg(radii, "rx", "ry", transform));
        }
    }

    public partial class OvalArc : IShape
    {
        public Paint paint;
        public Point<Internal> center;
        DPoint<Internal> _radii;
        public virtual DPoint<Internal> radii
        {
            get { return _radii; }
            set
            {
                if (value._dx < 0 || value._dy < 0)
                {
                    throw new InvalidOperationException("negative radius for OvalArc");
                }
                _radii = value;
            }
        }
        float _startAngle;
        public float startAngle
        {
            get { return _startAngle; }
            set
            {
                if (value <= -180 || 180 < value)
                {
                    throw new InvalidOperationException("invalid startAngle for OvalArc");
                }
                _startAngle = value;
            }
        }
        float _sweepAngle;
        public float sweepAngle
        {
            get { return _sweepAngle; }
            set
            {
                if (value < 0 || 360 < value)
                {
                    throw new InvalidOperationException("invalid sweepAngle for OvalArc");
                }
                _sweepAngle = value;
            }
        }
        public bool useCenter;
        public Point<Internal> start
        {
            get
            {
                var polar = Complex.Polar(startAngle);
                return center + new DPoint<Internal>(polar.re * radii._dx, polar.im * radii._dy);
            }
        }
        public Point<Internal> end
        {
            get
            {
                var polar = Complex.Polar(startAngle + sweepAngle);
                return center + new DPoint<Internal>(polar.re * radii._dx, polar.im * radii._dy);
            }
        }

        public OvalArc(Paint paint, Point<Internal> center, DPoint<Internal> radii, float startAngle, float sweepAngle, bool useCenter = true)
        {
            this.paint = paint; this.center = center; this.radii = radii; this.startAngle = startAngle; this.sweepAngle = sweepAngle; this.useCenter = useCenter;
        }

        public void AddSvg<X>(XElement element, Transform<Internal, X> transform) where X : External
        {
            var dString = string.Format("M {0} A {1} 0 {2} 0 {3}", start.Transform(transform), radii.Transform(transform), sweepAngle >= 180 ? 1 : 0, end.Transform(transform));
            if (useCenter)
            {
                dString += string.Format("L {0} L {1}", center, start);
            }
            element.Add(new XElement("path",
                new XAttribute("d", dString)));
        }
    }
}