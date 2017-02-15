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

        static Exception InvalidH()
        {
            return new InvalidOperationException("h is neither a value in [0, 360) nor NaN for Color");
        }
        static Exception InvalidS()
        {
            return new InvalidOperationException("s is not in [0, 100] for Color");
        }
        static Exception InvalidL()
        {
            return new InvalidOperationException("l is not in [0, 100] for Color");
        }
        public static Color Hsla(float h, float s, float l, byte a)
        {
            if (h < 0.0f || 360.0f <= h) throw InvalidH();
            if (float.IsNaN(s) || s < 0.0f || 100.0f < s) throw InvalidS();
            if (float.IsNaN(l) || l < 0.0f || 100.0f < l) throw InvalidL();

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

    public enum LineCap { Butt, Round, Square }
    public enum LineJoin { Miter, Round, Bevel }
    public enum FillRule { EvenOdd, Nonzero }
    public partial class Paint
    {
        public Color StrokeColor;
        public SizeEither StrokeWidth;

        public Color FillColor;

        LineCap _lineCap;
        public LineCap LineCap
        {
            get { return _lineCap; }
            set
            {
                if (!(0 <= value && (int)value < 3)) throw InvalidLineCap();
                _lineCap = value;
            }
        }
        LineJoin _lineJoin;
        public LineJoin LineJoin
        {
            get { return _lineJoin; }
            set
            {
                if (!(0 <= value && (int)value < 3)) throw InvalidLineJoin();
                _lineJoin = value;
            }
        }
        public float MiterLimit;

        FillRule _fillRule;
        public FillRule FillRule
        {
            get { return _fillRule; }
            set
            {
                if (!(0 <= value && (int)value < 2)) throw InvalidFillRule();
                _fillRule = value;
            }
        }

        static Exception InvalidLineCap()
        {
            return new InvalidOperationException("Invalid Line Cap for Paint");
        }
        static Exception InvalidLineJoin()
        {
            return new InvalidOperationException("Invalid Line Join for Paint");
        }
        static Exception InvalidFillRule()
        {
            return new InvalidOperationException("Invalid Fill Rule for Paint");
        }

        public Paint() { }
        public Paint(Color strokeColor, SizeEither strokeWidth,
            Color fillColor = default(Color),
            LineCap lineCap = LineCap.Butt, LineJoin lineJoin = LineJoin.Miter, float miterLimit = 4,
            FillRule fillRule = FillRule.EvenOdd)
        {
            StrokeColor = strokeColor; StrokeWidth = strokeWidth;
            FillColor = fillColor;
            LineCap = lineCap; LineJoin = lineJoin; MiterLimit = miterLimit;
            FillRule = fillRule;
        }
        public Paint(Paint paint)
            : this(paint.StrokeColor, paint.StrokeWidth,
                  paint.FillColor,
                  paint.LineCap, paint.LineJoin, paint.MiterLimit,
                  paint.FillRule)
        { }

        bool Equals(Paint paint)
        {
            return
                StrokeColor.Equals(paint.StrokeColor) &&
                StrokeWidth.Equals(paint.StrokeWidth) &&
                FillColor.Equals(paint.FillColor) &&
                LineCap == paint.LineCap &&
                LineJoin == paint.LineJoin &&
                MiterLimit == paint.MiterLimit &&
                FillRule == paint.FillRule;
        }

        /* internally W 150 x H 50 */
        public IShape LineCapLineJoinSample()
        {
            var points = Util.NewList<Point<Internal>>(
                new Point<Internal>(10, 10),
                new Point<Internal>(60, 10),
                new Point<Internal>(75, 40),
                new Point<Internal>(90, 10),
                new Point<Internal>(140, 10));
            var polylineBack = new Polyline(
                new Paint(Color.Rgba(0x808080FF), new SizeEither(10.0f, true), lineCap: LineCap, lineJoin: LineJoin),
                points);
            var polylineFront = new Polyline(
                new Paint(Color.Rgba(0xFFFFFFFF), new SizeEither(1.0f, true)),
                points);
            return new ShapeGroup(new IShape[] { polylineBack, polylineFront });
        }

        /* internally W 150 x H 100 */
        public IShape FillRuleSample()
        {
            return new Polyline(
                new Paint(Color.Rgba(0x404040FF), new SizeEither(3.0f, true), Color.Rgba(0x808080FF), fillRule: FillRule),
                Util.NewList<Point<Internal>>(
                new Point<Internal>(10, 90),
                new Point<Internal>(50, 10),
                new Point<Internal>(100, 60),
                new Point<Internal>(50, 60),
                new Point<Internal>(100, 10),
                new Point<Internal>(140, 90)),
                true);
        }
    }

    public static partial class Drawdata
    {
        public static XElement AddSvg(this XElement element, Paint paint, Transform<Internal, External> transform)
        {
            element.Add(
                new XAttribute("stroke", paint.StrokeColor.RgbaFunctionString()),
                new XAttribute("stroke-width", paint.StrokeWidth.Value(transform).ToString()),
                new XAttribute("fill", paint.FillColor.RgbaFunctionString()),
                new XAttribute("stroke-linecap", paint.LineCap.ToString().ToLower()),
                new XAttribute("stroke-linejoin", paint.LineJoin.ToString().ToLower()),
                new XAttribute("stroke-miterlimit", paint.MiterLimit),
                new XAttribute("fill-rule", paint.FillRule.ToString().ToLower()));
            return element;
        }
    }
}