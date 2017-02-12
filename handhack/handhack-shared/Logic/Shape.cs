using System;
using System.Xml.Linq;
using System.Collections.Generic;
using static System.Math;
using static handhack.GeometryStatic;
using static handhack.UtilStatic;

namespace handhack
{
    public partial interface IShape
    {
        void AddSvg(XElement element, Transform<Internal, External> transform);
    }
    public static partial class DrawdataStatic
    {
        public static XElement AddSvg(this XElement element, IShape shape, Transform<Internal, External> transform)
        {
            shape.AddSvg(element, transform);
            return element;
        }
    }

    public partial class ShapeGroup : IShape
    {
        public IShape[] shapes;

        public ShapeGroup(IShape[] shapes)
        {
            this.shapes = shapes;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            var gElement = new XElement("g");
            foreach (var shape in shapes)
            {
                gElement.AddSvg(shape, transform);
            }
            element.Add(gElement);
        }
    }

    public partial class Polyline : IShape
    {
        public Paint paint;
        public List<Point<Internal>> points;
        public Point<Internal> startPoint
        {
            get { return points[0]; }
            set { points[0] = value; }
        }
        public Point<Internal> endPoint
        {
            get { return points[points.Count - 1]; }
            set { points[points.Count - 1] = value; }
        }
        public bool closed;
        public bool bezier;

        public Polyline(Paint paint, List<Point<Internal>> points, bool closed = false, bool bezier = false)
        {
            this.paint = paint; this.points = points; this.closed = closed; this.bezier = bezier;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            if (points.Count >= 2)
            {
                var dString = "";
                dString += string.Format("M {0}", startPoint.Transform(transform));
                for (int i = 1; i < (!(closed && !bezier) ? points.Count : points.Count + 1); i++)
                {
                    if (!bezier)
                    {
                        dString += string.Format(" L {0}", points.LoopGet(i).Transform(transform));
                    }
                    else
                    {
                        var p0 = closed || i >= 2 ?
                            points.LoopGet(i - 2) :
                            startPoint;
                        var p1 = points.LoopGet(i - 1);
                        var p2 = points.LoopGet(i);
                        var p3 = closed || i < points.Count - 1 ?
                            points.LoopGet(i + 1) :
                            endPoint;
                        var conT = InterpolateCon(p0, p1, p2, p3).Transform(transform);
                        var trolT = InterpolateTrol(p0, p1, p2, p3).Transform(transform);
                        var toT = p2.Transform(transform);
                        dString += string.Format(" C {0} {1} {2}", conT, trolT, toT);
                    }
                }
                if (closed) dString += " Z";
                element.Add(new XElement(svgName("path"),
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
        public DPoint<Internal> radii { get { return _radii; } set { _radii = new DPoint<Internal>((float)Abs(value.dx), (float)Abs(value.dy)); } }

        public Oval(Paint paint, Point<Internal> center, DPoint<Internal> radii)
        {
            this.paint = paint; this.center = center; this.radii = radii;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            element.Add(new XElement(svgName("ellipse"))
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
        public DPoint<Internal> radii { get { return _radii; } set { _radii = new DPoint<Internal>((float)Abs(value.dx), (float)Abs(value.dy)); } }
        float _startAngle;
        public float startAngle
        {
            get { return _startAngle; }
            set
            {
                if (value <= -180 || 180 < value) throw new InvalidOperationException("invalid startAngle for OvalArc");
                _startAngle = value;
            }
        }
        float _sweepAngle;
        public float sweepAngle
        {
            get { return _sweepAngle; }
            set
            {
                if (value < 0 || 360 < value) throw new InvalidOperationException("invalid sweepAngle for OvalArc");
                _sweepAngle = value;
            }
        }
        public bool useCenter;
        public Point<Internal> startPoint
        {
            get
            {
                var polar = Complex.Polar(startAngle);
                return center + new DPoint<Internal>(polar.re * radii.dx, polar.im * radii.dy);
            }
        }
        public Point<Internal> endPoint
        {
            get
            {
                var polar = Complex.Polar(startAngle + sweepAngle);
                return center + new DPoint<Internal>(polar.re * radii.dx, polar.im * radii.dy);
            }
        }

        public OvalArc(Paint paint, Point<Internal> center, DPoint<Internal> radii, float startAngle, float sweepAngle, bool useCenter = true)
        {
            this.paint = paint; this.center = center; this.radii = radii; this.startAngle = startAngle; this.sweepAngle = sweepAngle; this.useCenter = useCenter;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            var dString = string.Format("M {0} A {1} 0 {2} 0 {3}", startPoint.Transform(transform), radii.Transform(transform), sweepAngle >= 180 ? 1 : 0, endPoint.Transform(transform));
            if (useCenter)
            {
                dString += string.Format(" L {0} L {1}", center, startPoint);
            }
            element.Add(new XElement(svgName("path"),
                new XAttribute("d", dString)));
        }
    }
}