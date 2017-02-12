using System;
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

        public string RgbaFunctionString()
        {
            return string.Format("rgba({0},{1},{2},{3})", r, g, b, a / 255.0f);
        }
    }

    public enum Linecap { Butt, Round, Square }
    public enum Linejoin { Miter, Round, Bevel }
    public enum FillRule { EvenOdd, Nonzero }
    public partial class Paint
    {
        public Color strokeColor;
        public SizeEither strokeWidth;
        public Color fillColor;
        Linecap _linecap;
        public Linecap linecap
        {
            get { return _linecap; }
            set
            {
                if (value < 0 || 3 <= (int)value) throw new InvalidOperationException("invalid strokelinecap for Paint");
                _linecap = value;
            }
        }
        Linejoin _linejoin;
        public Linejoin linejoin
        {
            get { return _linejoin; }
            set
            {
                if (value < 0 || 3 <= (int)value) throw new InvalidOperationException("invalid strokelinejoin for Paint");
                _linejoin = value;
            }
        }
        FillRule _fillRule;
        public FillRule fillRule
        {
            get { return _fillRule; }
            set
            {
                if (value < 0 || 2 <= (int)value) throw new InvalidOperationException("invalid strokelinejoin for Paint");
                _fillRule = value;
            }
        }

        public Paint(Color strokecolor, SizeEither strokewidth, Color fillcolor = default(Color),
            Linecap linecap = Linecap.Butt, Linejoin linejoin = Linejoin.Miter, FillRule fillRule = FillRule.EvenOdd)
        {
            this.strokeColor = strokecolor; this.strokeWidth = strokewidth; this.fillColor = fillcolor;
            this.linecap = linecap; this.linejoin = linejoin; this.fillRule = fillRule;
        }
        public Paint(Paint paint)
            : this(paint.strokeColor, paint.strokeWidth, paint.fillColor,
                  paint.linecap, paint.linejoin, paint.fillRule)
        { }
    }
    public static partial class PaintStatic
    {
        public static XElement AddSvg(this XElement element, Paint paint, Transform<Internal, External> transform)
        {
            element.Add(
                new XAttribute("stroke", paint.strokeColor.RgbaFunctionString()),
                new XAttribute("stroke-width", paint.strokeWidth.Value(transform).ToString()),
                new XAttribute("fill", paint.fillColor.RgbaFunctionString()),
                new XAttribute("stroke-linecap", paint.linecap.ToString().ToLower()),
                new XAttribute("stroke-linejoin", paint.linejoin.ToString().ToLower()),
                new XAttribute("fill-rule", paint.fillRule.ToString().ToLower()));
            return element;
        }
    }
}