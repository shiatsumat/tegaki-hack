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
        public Drawing Drawing;
        public List<Point<Internal>> Points;
        public bool Closed;
        public bool Bezier;

        public Polyline(Drawing drawing, List<Point<Internal>> points, bool closed = false, bool bezier = false)
        {
            Drawing = drawing; Points = points; Closed = closed; Bezier = bezier;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            if (Points.Count >= 2)
            {
                var dString = Bezier ? BezierDString(transform) : NonBezierDString(transform);
                element.Add(new XElement(Util.SvgName("path"),
                    new XAttribute("d", dString))
                    .AddSvg(Drawing, Closed, transform));
            }
        }
        string BezierDString(Transform<Internal, External> transform)
        {
            var bezierinfo = Points.ToBezier(Closed).Transform(transform);
            var res = string.Format("M {0}", bezierinfo.from);
            foreach (var controlto in bezierinfo.controltos)
            {
                res += string.Format(" C {0} {1} {2}", controlto.Con, controlto.Trol, controlto.To);
            }
            if (Closed) res += " Z";
            return res;
        }
        string NonBezierDString(Transform<Internal, External> transform)
        {
            var ps = Points.Transform(transform);
            var res = string.Format("M {0}", ps[0]);
            for (int i = 1; i < ps.Count; i++)
            {
                res += string.Format(" L {0}", ps[i]);
            }
            if (Closed) res += " Z";
            return res;
        }
    }

    public partial class Circle : IShape
    {
        public Drawing Drawing;
        public Point<Internal> Center;
        SizeEither _radius;
        public SizeEither Radius { get { return _radius; } set { _radius = new SizeEither(Math.Abs(value.Value), value.IsInternal); } }

        public Circle(Drawing drawing, Point<Internal> center, SizeEither radius)
        {
            Drawing = drawing; Center = center; Radius = radius;
        }

        public void AddSvg(XElement element, Transform<Internal, External> transform)
        {
            element.Add(new XElement(Util.SvgName("circle"))
                .AddSvg(Drawing, true, transform)
                .AddSvg(Center, "cx", "cy", transform)
                .AddSvg(Radius, "r", transform));
        }
    }
}