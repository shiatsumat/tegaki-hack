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
                var path = Bezier ? BezierPath(transform) : NonBezierPath(transform);
                if (Closed) canvas.DrawPath(path, Drawing.FillDrawing(transform));
                canvas.DrawPath(path, Drawing.StrokeDrawing(transform));
            }
        }
        Path BezierPath(Transform<Internal, External> transform)
        {
            var res = Drawing.NewPath();
            var bezierinfo = Points.ToBezier(Closed).Transform(transform);
            res.MoveTo(bezierinfo.from.X, bezierinfo.from.Y);
            foreach (var controlto in bezierinfo.controltos)
            {
                res.CubicTo(
                    controlto.Con.X, controlto.Con.Y,
                    controlto.Trol.X, controlto.Trol.Y,
                    controlto.To.X, controlto.To.Y);
            }
            if (Closed) res.Close();
            return res;
        }
        Path NonBezierPath(Transform<Internal, External> transform)
        {
            var ps = Points.Transform(transform);
            var res = Drawing.NewPath();
            res.MoveTo(ps[0].X, ps[0].Y);
            for (int i = 1; i < ps.Count; i++)
            {
                res.LineTo(ps[i].X, ps[i].Y);
            }
            if (Closed) res.Close();
            return res;
        }
    }

    public partial class Circle : IShape
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            var p = Center.Transform(transform);
            var r = Radius.Transform(transform);
            canvas.DrawCircle(p.X, p.Y, r, Drawing.FillDrawing(transform));
            canvas.DrawCircle(p.X, p.Y, r, Drawing.StrokeDrawing(transform));
        }
    }
}