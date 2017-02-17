using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace tegaki_hack
{
    public partial struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
        public uint Rgba
        {
            get { return ((uint)R << 24) + ((uint)G << 16) + ((uint)B << 8) + A; }
            set
            {
                R = (byte)((value & 0xFF000000) >> 24);
                G = (byte)((value & 0x00FF0000) >> 16);
                B = (byte)((value & 0x0000FF00) >> 8);
                A = (byte)(value & 0x000000FF);
            }
        }
        public float H
        {
            get
            {
                if (max == min) return float.NaN;
                else if (max == R) { var res = 60.0f * (G - B) / (max - min); return res >= 0 ? res : res + 360.0f; }
                else if (max == G) return 60.0f * (B - R) / (max - min) + 120.0f;
                else return 60.0f * (R - G) / (max - min) + 240.0f;
            }
        }
        public float S
        {
            get
            {
                if (cnt < Util.EPS || 255.0f - Util.EPS < cnt) return 0.0f;
                else if (cnt <= 127.5) return 100.0f * (cnt - min) / cnt;
                else return 100.0f * (max - cnt) / (255 - cnt);
            }
        }
        public float L
        {
            get { return cnt * 100.0f / 255.0f; }
        }
        byte max { get { return Math.Max(Math.Max(R, G), B); } }
        byte min { get { return Math.Min(Math.Min(R, G), B); } }
        float cnt { get { return (max + min) / 2.0f; } }

        Color(byte r, byte g, byte b, byte a)
        {
            R = r; G = g; B = b; A = a;
        }
        Color(uint rgba)
        {
            R = G = B = A = 0;
            Rgba = rgba;
        }

        public static Color Transparent = ByRgba(0, 0, 0, 0);

        public static Color ByRgba(byte r, byte g, byte b, byte a)
        {
            return new Color(r, g, b, a);
        }
        public static Color ByRgba(uint rgba)
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
        public static Color ByHsla(float h, float s, float l, byte a)
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
            return ByRgba(r, g, b, a);
        }

        public string RgbaFunctionString()
        {
            return string.Format("rgba({0},{1},{2},{3})", R, G, B, A / 255.0f);
        }
    }

    public enum LineCap { Butt, Round, Square }
    public enum LineJoin { Miter, Round, Bevel }
    public enum FillRule { EvenOdd, Nonzero }
    public partial class Drawing
    {
        public Color LineColor;
        public Color FillColor;

        public SizeEither LineWidth;

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
            return new InvalidOperationException("Invalid Line Cap for Drawing");
        }
        static Exception InvalidLineJoin()
        {
            return new InvalidOperationException("Invalid Line Join for Drawing");
        }
        static Exception InvalidFillRule()
        {
            return new InvalidOperationException("Invalid Fill Rule for Drawing");
        }

        public Drawing() { }
        public Drawing(Color lineColor, Color fillColor,
            SizeEither lineWidth,
            LineCap lineCap = LineCap.Butt, LineJoin lineJoin = LineJoin.Miter, float miterLimit = 4,
            FillRule fillRule = FillRule.EvenOdd)
        {
            LineColor = lineColor; FillColor = fillColor;
            LineWidth = lineWidth;
            LineCap = lineCap; LineJoin = lineJoin; MiterLimit = miterLimit;
            FillRule = fillRule;
        }
        public Drawing(Drawing drawing)
            : this(drawing.LineColor, drawing.FillColor,
                  drawing.LineWidth,
                  drawing.LineCap, drawing.LineJoin, drawing.MiterLimit,
                  drawing.FillRule)
        { }

        bool Equals(Drawing drawing)
        {
            return
                LineColor.Equals(drawing.LineColor) &&
                LineWidth.Equals(drawing.LineWidth) &&
                FillColor.Equals(drawing.FillColor) &&
                LineCap == drawing.LineCap &&
                LineJoin == drawing.LineJoin &&
                MiterLimit == drawing.MiterLimit &&
                FillRule == drawing.FillRule;
        }

        /* internally W 100 x H 100 */
        public IShape ColorSample()
        {
            return new Circle(new Drawing(LineColor, FillColor, new SizeEither(10.0f, true)),
                new Point<Internal>(50, 50), new SizeEither(40, true));
        }

        /* internally W 150 x H 50 */
        public IShape LineCapJoinSample()
        {
            var points = Util.NewList<Point<Internal>>(
                new Point<Internal>(10, 10),
                new Point<Internal>(60, 10),
                new Point<Internal>(75, 40),
                new Point<Internal>(90, 10),
                new Point<Internal>(140, 10));
            var polylineBack = new Polyline(
                new Drawing(Color.ByRgba(0x808080FF), Color.Transparent, new SizeEither(10.0f, true),
                    LineCap, LineJoin),
                points);
            var polylineFront = new Polyline(
                new Drawing(Color.ByRgba(0xFFFFFFFF), Color.Transparent, new SizeEither(1.0f, true)),
                points);
            return new ShapeGroup(new IShape[] { polylineBack, polylineFront });
        }

        /* internally W 150 x H 150 */
        public IShape MiterLimitSample()
        {
            var pointss = new List<Point<Internal>>[]
            {
                Util.NewList<Point<Internal>>(
                    new Point<Internal>(10, 10),
                    new Point<Internal>(50, 25),
                    new Point<Internal>(10, 40)),
                Util.NewList<Point<Internal>>(
                    new Point<Internal>(10, 60),
                    new Point<Internal>(75, 75),
                    new Point<Internal>(10, 90)),
                Util.NewList<Point<Internal>>(
                    new Point<Internal>(10, 110),
                    new Point<Internal>(100, 125),
                    new Point<Internal>(10, 140))
            };
            var shapes = new List<IShape>();
            foreach (var points in pointss)
            {
                shapes.Add(new Polyline(
                    new Drawing(Color.ByRgba(0x808080FF), Color.Transparent, new SizeEither(15.0f, true),
                        LineCap.Butt, LineJoin.Miter, MiterLimit),
                    points));
                shapes.Add(new Polyline(
                    new Drawing(Color.ByRgba(0xFFFFFFFF), Color.Transparent, new SizeEither(1.0f, true)),
                    points));
            }
            return new ShapeGroup(shapes.ToArray());
        }

        /* internally W 150 x H 100 */
        public IShape FillRuleSample()
        {
            return new Polyline(
                new Drawing(Color.ByRgba(0x404040FF), Color.ByRgba(0x808080FF), new SizeEither(3.0f, true), fillRule: FillRule),
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
        public static XElement AddSvg(this XElement element, Drawing drawing, bool fill, Transform<Internal, External> transform)
        {
            element.AddStrokeSvg(drawing, transform);
            element.AddFillSvg(drawing, fill);
            return element;
        }
        static XElement AddStrokeSvg(this XElement element, Drawing drawing, Transform<Internal, External> transform)
        {
            element.Add(
                new XAttribute("stroke", drawing.LineColor.RgbaFunctionString()),
                new XAttribute("stroke-width", drawing.LineWidth.Transform(transform).ToString()),
                new XAttribute("stroke-linecap", drawing.LineCap.ToString().ToLower()),
                new XAttribute("stroke-linejoin", drawing.LineJoin.ToString().ToLower()),
                new XAttribute("stroke-miterlimit", drawing.MiterLimit));
            return element;
        }
        static XElement AddFillSvg(this XElement element, Drawing drawing, bool fill)
        {
            if (fill)
                element.Add(
                    new XAttribute("fill", drawing.FillColor.RgbaFunctionString()),
                    new XAttribute("fill-rule", drawing.FillRule.ToString().ToLower()));
            else element.Add(new XAttribute("fill", "none"));
            return element;
        }
    }
}