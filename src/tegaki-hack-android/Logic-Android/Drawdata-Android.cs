using System;
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
            res.StrokeCap = linecap.ToNative();
            res.StrokeJoin = linejoin.ToNative();
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

    public static partial class DrawdataStatic
    {
        public static NativePaint.Cap ToNative(this Linecap linecap)
        {
            switch (linecap)
            {
                case Linecap.Butt:
                    return NativePaint.Cap.Butt;
                case Linecap.Round:
                    return NativePaint.Cap.Round;
                case Linecap.Square:
                    return NativePaint.Cap.Square;
                default:
                    throw new InvalidOperationException("invalid linecap");
            }
        }
        public static NativePaint.Join ToNative(this Linejoin linejoin)
        {
            switch (linejoin)
            {
                case Linejoin.Miter:
                    return NativePaint.Join.Miter;
                case Linejoin.Round:
                    return NativePaint.Join.Round;
                case Linejoin.Bevel:
                    return NativePaint.Join.Bevel;
                default:
                    throw new InvalidOperationException("invalid linejoin");
            }
        }
    }
}