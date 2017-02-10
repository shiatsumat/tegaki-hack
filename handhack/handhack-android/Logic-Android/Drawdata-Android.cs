using Android.Graphics;
using NativeColor = Android.Graphics.Color;
using NativePaint = Android.Graphics.Paint;

namespace handhack
{
    public partial interface IDrawable
    {
        void Draw<X>(Canvas canvas, Transform<Internal, X> transform) where X : External;
    }
    public static partial class DrawdataStatic
    {
        public static void Draw<X>(this Canvas canvas, IDrawable drawable, Transform<Internal, X> transform) where X : External
        {
            drawable.Draw(canvas, transform);
        }
    }

    public partial struct Color
    {
        public NativeColor native => new NativeColor(r, g, b, a);
    }

    public partial class Paint
    {
        public NativePaint strokePaint<X>(Transform<Internal, X> transform) where X : External
        {
            var res = new NativePaint();
            res.SetStyle(NativePaint.Style.Stroke);
            res.Color = strokecolor.native;
            res.StrokeWidth = strokewidth.value == 0 ? 1 : strokewidth.Transform(transform).value;
            switch (strokelinecap)
            {
                case Linecap.Butt:
                    res.StrokeCap = NativePaint.Cap.Butt;
                    break;
                case Linecap.Round:
                    res.StrokeCap = NativePaint.Cap.Round;
                    break;
                case Linecap.Square:
                    res.StrokeCap = NativePaint.Cap.Square;
                    break;
            }
            switch (strokelinejoin)
            {
                case Linejoin.Miter:
                    res.StrokeJoin = NativePaint.Join.Miter;
                    break;
                case Linejoin.Round:
                    res.StrokeJoin = NativePaint.Join.Round;
                    break;
                case Linejoin.Bevel:
                    res.StrokeJoin = NativePaint.Join.Bevel;
                    break;
            }
            return res;
        }
        public NativePaint fillPaint<X>(Transform<Internal, X> transform) where X : External
        {
            var res = new NativePaint();
            res.SetStyle(NativePaint.Style.Fill);
            res.Color = fillcolor.native;
            return res;
        }
    }
}