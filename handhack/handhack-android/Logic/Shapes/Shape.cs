using System.Xml;

namespace handhack
{
    public partial interface Shape
    {
        void AddSvg(XmlDocument svg, XmlNode node, Transform<Internal, External> transform);
    }
}