using System;
using System.Xml.Linq;

namespace handhack
{
    public partial class Freehand : Shape
    {
        public void AddSvg<X>(XElement element, Transform<Internal, X> transform)
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

        public void AddSvg<X>(XElement element, Transform<Internal, X> transform)
        {
            element.Add(new XElement("path",
                new XAttribute("d", string.Format("M {0} L {1}", start, end)))
                .AddSvg(paint, transform));
        }
    }
}