namespace handhack_android
{
    public struct Point<Pers>
	{
		public double x, y;
		public Complex complex => new Complex(x, y);

		public Point(double x, double y)
		{
			this.x = x; this.y = y;
		}
		public Point(Complex z)
		{
			this.x = z.re; this.y = z.im;
		}
		public static Point<Pers> operator +(Point<Pers> p, DPoint<Pers> v)
		{
			return new Point<Pers>(p.complex + v.complex);
		}
		public static Point<Pers> operator +(DPoint<Pers> v, Point<Pers> p)
		{
			return new Point<Pers>(v.complex + p.complex);
		}
		public static Point<Pers> operator -(Point<Pers> p, DPoint<Pers> v)
		{
			return new Point<Pers>(p.complex - v.complex);
		}
		public static DPoint<Pers> operator -(Point<Pers> p, Point<Pers> q)
		{
			return new DPoint<Pers>(p.complex - q.complex);
		}
		public static double distance(Point<Pers> p, Point<Pers> q)
		{
			return (p - q).norm;
		}
		public static Point<Pers> divide(Point<Pers> p, Point<Pers> q, double a, double b)
		{
			return new Point<Pers>((p.complex * b + q.complex * a) / (a + b));
		}
		public static Point<Pers> midpoint(Point<Pers> p, Point<Pers> q)
		{
			return divide(p, q, 1, 1);
		}

		public Point<Pers2> translate<Pers2>(Translation<Pers, Pers2> t)
		{
			return t.origin + new DPoint<Pers2>(this.x * t.scale.re, this.y * t.scale.im);
		}
		public Point<Pers2> untranslate<Pers2>(Translation<Pers2, Pers> t)
		{
			var p = this - t.origin;
			return new Point<Pers2>(p.dx / t.scale.re, p.dy / t.scale.im);
		}
	}

	public struct DPoint<Pers>
	{
		public double dx, dy;
		public Complex complex => new Complex(dx, dy);
		public double norm => complex.norm;

		public DPoint(double dx, double dy)
		{
			this.dx = dx; this.dy = dy;
		}
		public DPoint(Complex z)
		{
			this.dx = z.re; this.dy = z.im;
		}
		public static DPoint<Pers> operator +(DPoint<Pers> v, DPoint<Pers> w)
		{
			return new DPoint<Pers>(v.complex + w.complex);
		}
		public static DPoint<Pers> operator -(DPoint<Pers> v, DPoint<Pers> w)
		{
			return new DPoint<Pers>(v.complex - w.complex);
		}
		public static DPoint<Pers> operator *(double a, DPoint<Pers> v)
		{
			return new DPoint<Pers>(a * v.complex);
		}
		public static DPoint<Pers> operator *(DPoint<Pers> v, double a)
		{
			return new DPoint<Pers>(v.complex * a);
		}
		public static DPoint<Pers> operator /(DPoint<Pers> v, double a)
		{
			return new DPoint<Pers>(v.complex / a);
		}
	}

	public struct Translation<PersS, PersT>
	{
		public Point<PersT> origin;
		public Complex scale;
	}

	public struct Internal { }
	public struct External { }
}