using Android.Graphics;
using NativePaint = Android.Graphics.Paint;

namespace handhack
{
    public partial class Oval : Shape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            var p = center.Transform(transform);
            var r = radii.Transform(transform);
            canvas.DrawOval(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, paint.strokePaint(transform));
            canvas.DrawOval(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, paint.fillPaint(transform));
        }
    }

    public partial class OvalArc : Shape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            var p = center.Transform(transform);
            var r = radii.Transform(transform);
            canvas.DrawArc(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, startAngle, sweepAngle, useCenter, paint.strokePaint(transform));
            canvas.DrawArc(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, startAngle, sweepAngle, useCenter, paint.fillPaint(transform));
        }
    }
}