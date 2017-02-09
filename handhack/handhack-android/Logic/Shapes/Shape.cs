using System.Xml;

namespace handhack_android
{
    public partial interface Shape
    {
        void AddSvg(XmlDocument svg, XmlNode node, Transform<Internal, External> transform);
    }
}