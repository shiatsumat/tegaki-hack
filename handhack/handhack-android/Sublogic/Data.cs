using System;
using System.Xml;
using Android.Graphics;
using NativeColor = Android.Graphics.Color;
using NativePaint = Android.Graphics.Paint;

namespace handhack
{
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
            res.Color = strokecolor.native;
            res.StrokeWidth = strokewidth.Transform(transform).value;
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
        public NativePaint fillPaint(Transform<Internal, External> transform)
        {
            var res = new NativePaint();
            res.SetStyle(NativePaint.Style.Fill);
            res.Color = fillcolor.native;
            return res;
        }
    }
}