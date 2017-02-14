using System;
using System.Xml.Linq;

namespace tegaki_hack
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
        public float h
        {
            get
            {
                if (max == min) return float.NaN;
                else if (max == r) { var res = 60.0f * (g - b) / (max - min); return res >= 0 ? res : res + 360.0f; }
                else if (max == g) return 60.0f * (b - r) / (max - min) + 120.0f;
                else return 60.0f * (r - g) / (max - min) + 240.0f;
            }
        }
        public float s
        {
            get
            {
                if (cnt == 0.0f || cnt == 255.0f) return 0.0f;
                else if (cnt <= 127.5) return 100.0f * (cnt - min) / cnt;
                else return 100.0f * (max - cnt) / (255 - cnt);
            }
        }
        public float l
        {
            get { return cnt * 100.0f / 255.0f; }
        }
        byte max { get { return Math.Max(Math.Max(r, g), b); } }
        byte min { get { return Math.Min(Math.Min(r, g), b); } }
        float cnt { get { return (max + min) / 2.0f; } }

        Color(byte r, byte g, byte b, byte a)
        {
            this.r = r; this.g = g; this.b = b; this.a = a;
        }
        Color(uint rgba)
        {
            r = g = b = a = 0;
            this.rgba = rgba;
        }

        public static Color Rgba(byte r, byte g, byte b, byte a)
        {
            return new Color(r, g, b, a);
        }
        public static Color Rgba(uint rgba)
        {
            return new Color(rgba);
        }
        public static Color Hsla(float h, float s, float l, byte a)
        {
            if (h < 0.0f || 360.0f <= h) throw new InvalidOperationException("h is neither a value in [0, 360) nor NaN for Color");
            if (float.IsNaN(s) || s < 0.0f || 100.0f < s) throw new InvalidOperationException("s is not in [0, 100] for Color");
            if (float.IsNaN(l) || l < 0.0f || 100.0f < l) throw new InvalidOperationException("l is not in [0, 100] for Color");

            byte r = 0, g = 0, b = 0;

            if (float.IsNaN(h))
            {
                var v = (byte)(l * 255.0f / 100.0f);
                return new Color(v, v, v, a);
            }

            var max = 0.0f;
            var min = 0.0f;
            var sx = s / 100.0f;
            var lx = l / 100.0f;
            if (lx < 0.5f)
            {
                max = 255.0f * (lx + lx * sx);
                min = 255.0f * (lx - lx * sx);
            }
            else
            {
                max = 255.0f * (lx + (1 - lx) * sx);
                min = 255.0f * (lx - (1 - lx) * sx);
            }

            if (0 <= h && h < 60)
            {
                r = (byte)Math.Round(max);
                g = (byte)Math.Round((h / 60) * (max - min) + min);
                b = (byte)Math.Round(min);
            }
            else if (60 <= h && h < 120)
            {
                r = (byte)Math.Round(((120 - h) / 60) * (max - min) + min);
                g = (byte)Math.Round(max);
                b = (byte)Math.Round(min);
            }
            else if (120 <= h && h < 180)
            {
                r = (byte)Math.Round(min);
                g = (byte)Math.Round(max);
                b = (byte)Math.Round(((h - 120) / 60) * (max - min) + min);
            }
            else if (180 <= h && h < 240)
            {
                r = (byte)Math.Round(min);
                g = (byte)Math.Round(((240 - h) / 60) * (max - min) + min);
                b = (byte)Math.Round(max);
            }
            else if (240 <= h && h < 300)
            {
                r = (byte)Math.Round(((h - 240) / 60) * (max - min) + min);
                g = (byte)Math.Round(min);
                b = (byte)Math.Round(max);
            }
            else
            {
                r = (byte)Math.Round(max);
                g = (byte)Math.Round(min);
                b = (byte)Math.Round(((360 - h) / 60) * (max - min) + min);
            }
            return Rgba(r, g, b, a);
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
            strokeColor = strokecolor; strokeWidth = strokewidth; fillColor = fillcolor;
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