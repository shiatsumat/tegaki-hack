using System;
using System.Xml;

namespace handhack
{
    public partial struct Color
    {
        public uint rgba;
        public byte r { get { return (byte)((rgba & 0xFF000000) >> 24); } set { rgba = (rgba & 0x00FFFFFF) | ((uint)value << 24); } }
        public byte g { get { return (byte)((rgba & 0xFF000000) >> 16); } set { rgba = (rgba & 0x00FFFFFF) | ((uint)value << 16); } }
        public byte b { get { return (byte)((rgba & 0xFF000000) >> 8); } set { rgba = (rgba & 0x00FFFFFF) | ((uint)value << 8); } }
        public byte a { get { return (byte)((rgba & 0xFF000000) >> 0); } set { rgba = (rgba & 0x00FFFFFF) | ((uint)value << 0); } }

        public Color(byte r, byte g, byte b, byte a)
        {
            rgba = 0;
            this.r = r; this.g = g; this.b = b; this.a = a;
        }

        public override string ToString()
        {
            return string.Format("#{0:X}", rgba);
        }
    }

    public partial class Paint
    {
        public Color strokecolor, fillcolor;
        public Size<Internal> strokewidth;
        public enum Linecap { Butt, Round, Square }
        Linecap _strokelinecap;
        public Linecap strokelinecap
        {
            get { return _strokelinecap; }
            set
            {
                if (value < 0 || 3 <= (int)value)
                {
                    throw new InvalidOperationException("invalid strokelinecap for Paint");
                }
                _strokelinecap = value;
            }
        }
        public enum Linejoin { Miter, Round, Bevel }
        Linejoin _strokelinejoin;
        public Linejoin strokelinejoin
        {
            get { return _strokelinejoin; }
            set
            {
                if (value < 0 || 3 <= (int)value)
                {
                    throw new InvalidOperationException("invalid strokelinejoin for Paint");
                }
                _strokelinejoin = value;
            }
        }

        public Paint(Color strokecolor, Size<Internal> strokewidth, Color fillcolor,
            Linecap strokelinecap, Linejoin strokelinejoin)
        {
            this.strokecolor = strokecolor; this.strokewidth = strokewidth; this.fillcolor = fillcolor;
            this.strokelinecap = strokelinecap; this.strokelinejoin = strokelinejoin;
        }

        public void AddSvg(XmlDocument svg, XmlNode node, Transform<Internal, External> transform)
        {
            var strokecolorAttribute = svg.CreateAttribute("stroke-color");
            strokecolorAttribute.Value = strokecolor.ToString();
            node.Attributes.Append(strokecolorAttribute);
            var strokewidthAttribute = svg.CreateAttribute("stroke");
            strokewidthAttribute.Value = strokewidth.Transform(transform).value.ToString();
            node.Attributes.Append(strokewidthAttribute);
            var fillcolorAttribute = svg.CreateAttribute("fill-color");
            fillcolorAttribute.Value = fillcolor.ToString();
            node.Attributes.Append(fillcolorAttribute);
        }
    }

    public static partial class Shapes
    {
        public enum Shapesort { Linelike, Squarelike, Circlelike, Textlike }
        public const int shapesortNumber = 4;
        public static readonly int[] shapeNumbers = new int[] { 3, 4, 3, 2 };

        public enum Linelikeshape { Freehand, Line, Goodline }
        public enum Squarelikeshape { Square, Roundsquare, Rectangle, Roundrectangle }
        public enum Circlelikeshape { Circle, Oval, Arc }
        public enum Textlikeshape { Text, Fancytext }

        public static int NextShape(Shapesort shapesort, int nowshape)
        {
            return (nowshape + 1) % shapeNumbers[(int)shapesort];
        }
    }
}