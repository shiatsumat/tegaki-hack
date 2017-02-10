using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace handhack
{
    public partial struct Color
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
        public uint rgba
        {
            get { return ((uint)r << 24) + ((uint)g << 16) + ((uint)b << 8) + (uint)a; }
            set
            {
                r = (byte)((value & 0xFF000000) >> 24);
                g = (byte)((value & 0x00FF0000) >> 16);
                b = (byte)((value & 0x0000FF00) >> 8);
                a = (byte)(value & 0x000000FF);
            }
        }

        public Color(uint rgba)
        {
            r = g = b = a = 0;
            this.rgba = rgba;
        }
        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = r; this.g = g; this.b = b; this.a = a;
        }

        public override string ToString()
        {
            return string.Format("#{0:X}", rgba);
        }
    }

    public enum Linecap { Butt, Round, Square }
    public enum Linejoin { Miter, Round, Bevel }
    public partial class Paint
    {
        public Color strokecolor, fillcolor;
        public SizeEither strokewidth;
        Linecap _strokelinecap;
        public Linecap strokelinecap
        {
            get { return _strokelinecap; }
            set
            {
                if (value < 0 || 3 <= (int)value) throw new InvalidOperationException("invalid strokelinecap for Paint");
                _strokelinecap = value;
            }
        }
        Linejoin _strokelinejoin;
        public Linejoin strokelinejoin
        {
            get { return _strokelinejoin; }
            set
            {
                if (value < 0 || 3 <= (int)value) throw new InvalidOperationException("invalid strokelinejoin for Paint");
                _strokelinejoin = value;
            }
        }

        public Paint(Color strokecolor, SizeEither strokewidth, Color fillcolor = default(Color),
            Linecap strokelinecap = Linecap.Butt, Linejoin strokelinejoin = Linejoin.Miter)
        {
            this.strokecolor = strokecolor; this.strokewidth = strokewidth; this.fillcolor = fillcolor;
            this.strokelinecap = strokelinecap; this.strokelinejoin = strokelinejoin;
        }
        public Paint(Paint paint)
            : this(paint.strokecolor, paint.strokewidth, paint.fillcolor,
                  paint.strokelinecap, paint.strokelinejoin)
        { }
    }
    public static partial class PaintStatic
    {
        public static XElement AddSvg(this XElement element, Paint paint, Transform<Internal, External> transform)
        {
            element.Add(
                new XAttribute("stroke-color", paint.strokecolor.ToString()),
                new XAttribute("stroke", paint.strokewidth.Value(transform).ToString()),
                new XAttribute("fill-color", paint.fillcolor.ToString()),
                new XAttribute("stroke-linecap", paint.strokelinecap.ToString().ToLower()),
                new XAttribute("stroke-linejoin", paint.strokelinejoin.ToString().ToLower()));
            return element;
        }
    }
}