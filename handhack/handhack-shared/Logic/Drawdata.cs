using System;
using System.Xml.Linq;

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
    }
    public static partial class PaintStatic
    {
        public static XElement AddSvg<X>(this XElement element, Paint paint, Transform<Internal, X> transform)
        {
            element.Add(
                new XAttribute("stroke-color", paint.strokecolor.ToString()),
                new XAttribute("stroke", paint.strokewidth.Transform(transform).value.ToString()),
                new XAttribute("fill-color", paint.fillcolor.ToString()),
                new XAttribute("stroke-linecap", paint.strokelinecap.ToString().ToLower()),
                new XAttribute("stroke-linejoin", paint.strokelinejoin.ToString().ToLower()));
            return element;
        }
    }
}