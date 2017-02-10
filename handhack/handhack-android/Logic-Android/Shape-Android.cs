using Android.Graphics;
using static handhack.GeometryStatic;

namespace handhack
{
    public partial class ShapeGroup : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            foreach (var shape in shapes)
            {
                canvas.Draw(shape, transform);
            }
        }
    }

    public partial class Polyline : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            if (points.Count >= 2)
            {
                var path = new Path();
                var startt = startPoint.Transform(transform);
                path.MoveTo(startt.x, startt.y);
                for (int i = 1; closed ? i < points.Count + 1 : i < points.Count; i++)
                {
                    if (!bezier)
                    {
                        var pt = points.LoopGet(i).Transform(transform);
                        path.LineTo(pt.x, pt.y);
                    }
                    else
                    {
                        var p0 = closed || i >= 2 ?
                            points.LoopGet(i - 2) :
                            startPoint;
                        var p1 = points.LoopGet(i - 1);
                        var p2 = points.LoopGet(i);
                        var p3 = closed || i < points.Count - 1 ?
                            points.LoopGet(i + 1) :
                            endPoint;
                        var conT = InterpolateCon(p0, p1, p2, p3).Transform(transform);
                        var trolT = InterpolateTrol(p0, p1, p2, p3).Transform(transform);
                        var toT = p2.Transform(transform);
                        path.CubicTo(conT.x, conT.y, trolT.x, trolT.y, toT.x, toT.y);
                    }
                }
                canvas.DrawPath(path, paint.strokePaint(transform));
                canvas.DrawPath(path, paint.fillPaint(transform));
            }
        }
    }

    public partial class Oval : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            var p = center.Transform(transform);
            var r = radii.Transform(transform);
            canvas.DrawOval(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, paint.strokePaint(transform));
            canvas.DrawOval(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, paint.fillPaint(transform));
        }
    }

    public partial class OvalArc : IShape
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