using Android.Graphics;
using NativeColor = Android.Graphics.Color;
using NativePaint = Android.Graphics.Paint;

namespace tegaki_hack
{

    public partial interface IShape
    {
        void Draw(Canvas canvas, Transform<Internal, External> transform);
    }

    public partial struct Color
    {
        public NativeColor ToNative() { return new NativeColor(R, G, B, A); }
    }

    public partial class Paint
    {
        public NativePaint.Cap NativeLineCap
        {
            get
            {
                switch (LineCap)
                {
                    case LineCap.Butt:
                        return NativePaint.Cap.Butt;
                    case LineCap.Round:
                        return NativePaint.Cap.Round;
                    case LineCap.Square:
                        return NativePaint.Cap.Square;
                    default:
                        throw InvalidLineCap();
                }
            }
        }

        public NativePaint.Join NativeLineJoin
        {
            get
            {
                switch (LineJoin)
                {
                    case LineJoin.Miter:
                        return NativePaint.Join.Miter;
                    case LineJoin.Round:
                        return NativePaint.Join.Round;
                    case LineJoin.Bevel:
                        return NativePaint.Join.Bevel;
                    default:
                        throw InvalidLineJoin();
                }
            }
        }

        public NativePaint StrokePaint(Transform<Internal, External> transform)
        {
            var res = new NativePaint();
            res.SetStyle(NativePaint.Style.Stroke);
            res.Color = LineColor.ToNative();
            res.StrokeWidth = LineWidth.Transform(transform);
            res.StrokeCap = NativeLineCap;
            res.StrokeJoin = NativeLineJoin;
            res.StrokeMiter = MiterLimit;
            return res;
        }

        public NativePaint FillPaint(Transform<Internal, External> transform)
        {
            var res = new NativePaint();
            res.SetStyle(NativePaint.Style.Fill);
            res.Color = FillColor.ToNative();
            return res;
        }

        public Path NewPath()
        {
            var path = new Path();
            switch (FillRule)
            {
                case FillRule.EvenOdd:
                    path.SetFillType(Path.FillType.EvenOdd);
                    break;
                case FillRule.Nonzero:
                    path.SetFillType(Path.FillType.Winding);
                    break;
                default:
                    throw InvalidFillRule();
            }
            return path;
        }
    }

}