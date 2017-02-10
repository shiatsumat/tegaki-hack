using System.Xml.Linq;

namespace handhack
{
    public partial interface Shape
    {
        void AddSvg<X>(XElement element, Transform<Internal, X> transform);
    }
    public static partial class ShapeStatic
    {
        public static XElement AddSvg<X>(this XElement element, Shape shape, Transform<Internal, X> transform)
        {
            shape.AddSvg(element, transform);
            return element;
        }
    }
}