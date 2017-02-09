using System.Xml;
using NativeColor = Android.Graphics.Color;
using NativePaint = Android.Graphics.Paint;

namespace handhack_android
{
    public partial struct Color
    {
        public NativeColor native => new NativeColor(r, g, b, a);
    }

    public partial struct Paint
    {
        public NativePaint strokePaint(Transform<Internal, External> transform)
        {
            var res = new NativePaint();
            res.SetStyle(NativePaint.Style.Stroke);
            res.Color = strokecolor.native;
            res.StrokeWidth = strokewidth.Transform(transform).value;
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