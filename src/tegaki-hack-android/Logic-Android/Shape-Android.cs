using Android.Graphics;
using static tegaki_hack.GeometryStatic;

namespace tegaki_hack
{
    public partial class ShapeGroup : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            foreach (var shape in shapes)
            {
                shape.Draw(canvas, transform);
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
                switch (paint.fillRule)
                {
                    case FillRule.EvenOdd:
                        path.SetFillType(Path.FillType.EvenOdd);
                        break;
                    case FillRule.Nonzero:
                        path.SetFillType(Path.FillType.Winding);
                        break;
                }
                var startt = startPoint.Transform(transform);
                path.MoveTo(startt.x, startt.y);
                for (int i = 1; i < (closed && bezier ? points.Count + 1 : points.Count); i++)
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
                if (closed) path.Close();
                canvas.DrawPath(path, paint.fillPaint(transform));
                canvas.DrawPath(path, paint.strokePaint(transform));
            }
        }
    }

    public partial class Oval : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            var p = center.Transform(transform);
            var r = radii.Transform(transform);
            canvas.DrawOval(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, paint.fillPaint(transform));
            canvas.DrawOval(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, paint.strokePaint(transform));
        }
    }

    public partial class OvalArc : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            var p = center.Transform(transform);
            var r = radii.Transform(transform);
            canvas.DrawArc(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, startAngle, sweepAngle, useCenter, paint.fillPaint(transform));
            canvas.DrawArc(p.x - r.dx, p.y - r.dy, p.x + r.dx, p.y + r.dy, startAngle, sweepAngle, useCenter, paint.strokePaint(transform));
        }
    }
}