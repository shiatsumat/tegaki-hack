using Android.Graphics;
using NativeColor = Android.Graphics.Color;

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

    public partial class Drawing
    {
        public Paint.Cap NativeLineCap
        {
            get
            {
                switch (LineCap)
                {
                    case LineCap.Butt:
                        return Paint.Cap.Butt;
                    case LineCap.Round:
                        return Paint.Cap.Round;
                    case LineCap.Square:
                        return Paint.Cap.Square;
                    default:
                        throw InvalidLineCap();
                }
            }
        }

        public Paint.Join NativeLineJoin
        {
            get
            {
                switch (LineJoin)
                {
                    case LineJoin.Miter:
                        return Paint.Join.Miter;
                    case LineJoin.Round:
                        return Paint.Join.Round;
                    case LineJoin.Bevel:
                        return Paint.Join.Bevel;
                    default:
                        throw InvalidLineJoin();
                }
            }
        }

        public Paint StrokeDrawing(Transform<Internal, External> transform)
        {
            var res = new Paint();
            res.SetStyle(Paint.Style.Stroke);
            res.Color = LineColor.ToNative();
            res.StrokeWidth = LineWidth.Transform(transform);
            res.StrokeCap = NativeLineCap;
            res.StrokeJoin = NativeLineJoin;
            res.StrokeMiter = MiterLimit;
            return res;
        }

        public Paint FillDrawing(Transform<Internal, External> transform)
        {
            var res = new Paint();
            res.SetStyle(Paint.Style.Fill);
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