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
        public NativeColor native => new NativeColor(r, g, b, a);
    }

    public partial class Paint
    {
        public NativePaint strokePaint(Transform<Internal, External> transform)
        {
            var res = new NativePaint();
            res.SetStyle(NativePaint.Style.Stroke);
            res.Color = strokeColor.native;
            res.StrokeWidth = strokeWidth.Value(transform);
            switch (linecap)
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
            switch (linejoin)
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
        public NativePaint fillPaint(Transform<Internal, External> transform)
        {
            var res = new NativePaint();
            res.SetStyle(NativePaint.Style.Fill);
            res.Color = fillColor.native;
            return res;
        }
    }
}