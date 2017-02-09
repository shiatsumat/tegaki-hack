using System;
using System.Xml;
using Android.Graphics;

namespace handhack_android
{
    public partial class Freehand : Shape
    {
        public void AddSvg(XmlDocument svg, XmlNode node, Transform<Internal, External> transform)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Line : Shape
    {
        public Paint paint;
        public Point<Internal> start, end;

        public Line(Paint paint, Point<Internal> start, Point<Internal> end)
        {
            this.paint = paint; this.start = start; this.end = end;
        }

        public void AddSvg(XmlDocument svg, XmlNode node, Transform<Internal, External> transform)
        {
            var path = svg.CreateElement("path");
            var d = svg.CreateAttribute("d");
            d.Value = string.Format("M {0} L {1}", start, end);
            path.Attributes.Append(d);
            paint.AddSvg(svg, path, transform);
            node.AppendChild(path);
        }
    }
}