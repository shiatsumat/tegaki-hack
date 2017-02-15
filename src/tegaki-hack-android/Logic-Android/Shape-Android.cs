using Android.Graphics;

namespace tegaki_hack
{
    public partial class ShapeGroup : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            foreach (var shape in Shapes)
            {
                shape.Draw(canvas, transform);
            }
        }
    }

    public partial class Polyline : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            if (Points.Count >= 2)
            {
                var path = Paint.NewPath();
                if (Bezier)
                {
                    var bezierinfo = Points.ToBezier(Closed).Transform(transform);
                    path.MoveTo(bezierinfo.from.X, bezierinfo.from.Y);
                    foreach (var controlto in bezierinfo.controltos)
                    {
                        path.CubicTo(
                            controlto.Con.X, controlto.Con.Y,
                            controlto.Trol.X, controlto.Trol.Y,
                            controlto.To.X, controlto.To.Y);
                    }
                }
                else
                {
                    var ps = Points.Transform(transform);
                    for (int i = 0; i < ps.Count; i++)
                    {
                        var p = ps[i];
                        if (i == 0) path.MoveTo(p.X, p.Y);
                        else path.LineTo(p.X, p.Y);
                    }
                }
                if (Closed) path.Close();
                canvas.DrawPath(path, Paint.fillPaint(transform));
                canvas.DrawPath(path, Paint.strokePaint(transform));
            }
        }
    }

    public partial class Oval : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            var p = Center.Transform(transform);
            var r = Radii.Transform(transform);
            canvas.DrawOval(p.X - r.Dx, p.Y - r.Dy, p.X + r.Dx, p.Y + r.Dy, Paint.fillPaint(transform));
            canvas.DrawOval(p.X - r.Dx, p.Y - r.Dy, p.X + r.Dx, p.Y + r.Dy, Paint.strokePaint(transform));
        }
    }

    public partial class OvalArc : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            var p = Center.Transform(transform);
            var r = Radii.Transform(transform);
            canvas.DrawArc(p.X - r.Dx, p.Y - r.Dy, p.X + r.Dx, p.Y + r.Dy, StartAngle, SweepAngle, UseCenter, Paint.fillPaint(transform));
            canvas.DrawArc(p.X - r.Dx, p.Y - r.Dy, p.X + r.Dx, p.Y + r.Dy, StartAngle, SweepAngle, UseCenter, Paint.strokePaint(transform));
        }
    }
}