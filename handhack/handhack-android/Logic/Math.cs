using System;

namespace handhack_android
{
	public struct Point
	{
		public double x, y;

		public Point(double x, double y)
		{
			this.x = x; this.y = y;
		}
		public static Point operator+(Point p, DPoint v)
		{
			return new Point(p.x + v.dx, p.y + v.dy);
		}
		public static Point operator+(DPoint v, Point p)
		{
			return new Point(v.dx + p.x, v.dy + p.y);
		}
		public static Point operator-(Point p, DPoint v)
		{
			return new Point(p.x - v.dx, p.y - v.dy);
		}
		public static DPoint operator-(Point p, Point q)
		{
			return new DPoint(p.x - q.x, p.y - q.y);
		} 
		public static double distance(Point p, Point q)
		{
			return (p - q).norm;
		}
		public static Point divide(Point p, Point q, double a, double b)
		{
			return new Point((p.x * b + q.x + a) / (a + b), (p.y * b + q.y + a) / (a + b));
		}
	}

	public struct DPoint
	{
		public double dx, dy;

		public DPoint(double dx, double dy)
		{
			this.dx = dx; this.dy = dy;
		}
		public static DPoint operator+(DPoint v, DPoint w)
		{
			return new DPoint(v.dx + w.dx, v.dy + w.dy);
		}
		public static DPoint operator-(DPoint v, DPoint w)
		{
			return new DPoint(v.dx - w.dx, v.dy - w.dy);
		}
		public static DPoint operator *(double a, DPoint v)
		{
			return new DPoint(a * v.dx, a * v.dy);
		}
		public static DPoint operator*(DPoint v, double a)
		{
			return new DPoint(v.dx * a, v.dy * a);
		}
		public static DPoint operator /(DPoint v, double a)
		{
			return new DPoint(v.dx / a, v.dy / a);
		}
		public double norm { get { return Math.Sqrt(dx * dx + dy * dy); }  }
	}
}