using System;
using System.Xml;

namespace handhack_android
{
    public partial class Oval : Shape
    {
        public Paint paint;
        public Point<Internal> center;
        DPoint<Internal> _radii;
        public virtual DPoint<Internal> radii
        {
            get { return _radii; }
            set
            {
                if (value.dx < 0 || value.dy < 0)
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

        public void AddSvg(XmlDocument svg, XmlNode node, Transform<Internal, External> transform)
        {
            var ellipse = svg.CreateElement("ellipse");
            paint.AddSvg(svg, ellipse, transform);
            center.AddSvg(svg, ellipse, "cx", "cy", transform);
            radii.AddSvg(svg, ellipse, "rx", "ry", transform);
            node.AppendChild(ellipse);
        }
    }

    public partial class OvalArc : Shape
    {
        public Paint paint;
        public Point<Internal> center;
        DPoint<Internal> _radii;
        public virtual DPoint<Internal> radii
        {
            get { return _radii; }
            set
            {
                if (value.dx < 0 || value.dy < 0)
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
                return center + new DPoint<Internal>(polar.re * radii.dx, polar.im * radii.dy);
            }
        }
        public Point<Internal> end
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

        public void AddSvg(XmlDocument svg, XmlNode node, Transform<Internal, External> transform)
        {
            var path = svg.CreateElement("path");
            var d = svg.CreateAttribute("d");
            d.Value = string.Format("M {0} A {1} 0 {2} 0 {3}", start.Transform(transform), radii.Transform(transform), sweepAngle >= 180 ? 1 : 0, end.Transform(transform));
            if (useCenter)
            {
                d.Value += string.Format("L {0} L {1}", center, start);
            }
            path.Attributes.Append(d);
            paint.AddSvg(svg, path, transform);
            node.AppendChild(path);
        }
    }
}