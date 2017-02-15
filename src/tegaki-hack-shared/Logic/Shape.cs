using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace tegaki_hack
{
    public partial interface IShape
    {
        void AddSvg(XElement element, Transform<Internal, External> transform);
    }
    public static partial class Shape
    {
        public static XElement AddSvg(this XElement element, IShape shape, Transform<Internal, External> transform)
        {
            shape.AddSvg(element, transform);
            return element;
        }
    }

    public partial class ShapeGroup : IShape
    {
        public IShape[] Shapes;

        public ShapeGroup(IShape[] shapes)
        {
            Shapes = shapes;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            var gElement = new XElement(Util.SvgName("g"));
            foreach (var shape in Shapes)
            {
                gElement.AddSvg(shape, transform);
            }
            element.Add(gElement);
        }
    }

    public partial class Polyline : IShape
    {
        public Paint Paint;
        public List<Point<Internal>> Points;
        public bool Closed;
        public bool Bezier;

        public Polyline(Paint paint, List<Point<Internal>> points, bool closed = false, bool bezier = false)
        {
            Paint = paint; Points = points; Closed = closed; Bezier = bezier;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            if (Points.Count >= 2)
            {
                var dString = "";
                if (Bezier)
                {
                    var bezierinfo = Points.ToBezier(Closed).Transform(transform);
                    dString += string.Format("M {0}", bezierinfo.from);
                    foreach (var controlto in bezierinfo.controltos)
                    {
                        dString += string.Format(" C {0} {1} {2}", controlto.Con, controlto.Trol, controlto.To);
                    }
                }
                else
                {
                    var ps = Points.Transform(transform);
                    for (int i = 0; i < ps.Count; i++)
                    {
                        var p = ps[i];
                        if (i == 0) dString += string.Format("M {0}", p);
                        else dString += string.Format(" L {0}", p);
                    }
                }
                if (Closed) dString += " Z";
                element.Add(new XElement(Util.SvgName("path"),
                    new XAttribute("d", dString))
                    .AddSvg(Paint, transform));
            }
        }
    }

    public partial class Oval : IShape
    {
        public Paint Paint;
        public Point<Internal> Center;
        DPoint<Internal> _radii;
        public DPoint<Internal> Radii { get { return _radii; } set { _radii = new DPoint<Internal>(Math.Abs(value.Dx), Math.Abs(value.Dy)); } }

        public Oval(Paint paint, Point<Internal> center, DPoint<Internal> radii)
        {
            Paint = paint; Center = center; Radii = radii;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            element.Add(new XElement(Util.SvgName("ellipse"))
                .AddSvg(Paint, transform)
                .AddSvg(Center, "cx", "cy", transform)
                .AddSvg(Radii, "rx", "ry", transform));
        }
    }

    public partial class OvalArc : IShape
    {
        public Paint Paint;
        public Point<Internal> Center;
        DPoint<Internal> _radii;
        public DPoint<Internal> Radii { get { return _radii; } set { _radii = new DPoint<Internal>(Math.Abs(value.Dx), Math.Abs(value.Dy)); } }
        float _startAngle;
        public float StartAngle
        {
            get { return _startAngle; }
            set
            {
                if (value <= -180 || 180 < value) throw new InvalidOperationException("invalid startAngle for OvalArc");
                _startAngle = value;
            }
        }
        float _sweepAngle;
        public float SweepAngle
        {
            get { return _sweepAngle; }
            set
            {
                if (value < 0 || 360 < value) throw new InvalidOperationException("invalid sweepAngle for OvalArc");
                _sweepAngle = value;
            }
        }
        public bool UseCenter;
        public Point<Internal> StartPoint
        {
            get
            {
                var polar = Complex.Polar(StartAngle);
                return Center + new DPoint<Internal>(polar.Re * Radii.Dx, polar.Im * Radii.Dy);
            }
        }
        public Point<Internal> EndPoint
        {
            get
            {
                var polar = Complex.Polar(StartAngle + SweepAngle);
                return Center + new DPoint<Internal>(polar.Re * Radii.Dx, polar.Im * Radii.Dy);
            }
        }

        public OvalArc(Paint paint, Point<Internal> center, DPoint<Internal> radii, float startAngle, float sweepAngle, bool useCenter = true)
        {
            Paint = paint; Center = center; Radii = radii; StartAngle = startAngle; SweepAngle = sweepAngle; UseCenter = useCenter;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            var dString = string.Format("M {0} A {1} 0 {2} 0 {3}", StartPoint.Transform(transform), Radii.Transform(transform), SweepAngle >= 180 ? 1 : 0, EndPoint.Transform(transform));
            if (UseCenter)
            {
                dString += string.Format(" L {0} L {1}", Center, StartPoint);
            }
            element.Add(new XElement(Util.SvgName("path"),
                new XAttribute("d", dString)));
        }
    }
}