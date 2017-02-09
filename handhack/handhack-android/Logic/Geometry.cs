using System.Xml;

namespace handhack
{
    public partial struct Point<Pers>
	{
		public float x, y;
		public Complex complex => new Complex(x, y);
        public Size<Pers> X { get { return new Size<Pers>(x); } set { x = value.value; } }
        public Size<Pers> Y { get { return new Size<Pers>(y); } set { y = value.value; } }

        public Point(float x, float y)
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
		public static float distance(Point<Pers> p, Point<Pers> q)
		{
			return (p - q).norm;
		}
		public static Point<Pers> divide(Point<Pers> p, Point<Pers> q, float a, float b)
		{
			return new Point<Pers>((p.complex * b + q.complex * a) / (a + b));
		}
		public static Point<Pers> midpoint(Point<Pers> p, Point<Pers> q)
		{
			return divide(p, q, 1, 1);
		}

		public Point<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform)
		{
            var v = this - transform.originS;
			return transform.originT + new DPoint<Pers2>(v.dx * transform.scalex, v.dy * transform.scaley);
		}
		public Point<Pers2> Untransform<Pers2>(Transform<Pers2, Pers> transform)
		{
			var v = this - transform.originT;
			return transform.originS + new DPoint<Pers2>(v.dx / transform.scalex, v.dy / transform.scaley);
        }

        public void AddSvg<Pers2>(XmlDocument svg, XmlNode node, string xname, string yname, Transform<Pers, Pers2> transform)
        {
            var p = Transform(transform);

            var xAttribute = svg.CreateAttribute(xname);
            xAttribute.Value = p.x.ToString();
            svg.AppendChild(xAttribute);

            var yAttribute = svg.CreateAttribute(yname);
            yAttribute.Value = p.y.ToString();
            svg.AppendChild(yAttribute);
        }

        override public string ToString()
        {
            return string.Format("{0} {1}", x, y);
        }
    }

	public partial struct DPoint<Pers>
	{
		public float dx, dy;
		public Complex complex => new Complex(dx, dy);
		public float norm => complex.norm;
        public float arg => complex.arg;
        public Size<Pers> DX { get { return new Size<Pers>(dx); } set { dx = value.value; } }
        public Size<Pers> DY { get { return new Size<Pers>(dy); } set { dy = value.value; } }

        public DPoint(float dx, float dy)
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
		public static DPoint<Pers> operator *(float a, DPoint<Pers> v)
		{
			return new DPoint<Pers>(a * v.complex);
		}
		public static DPoint<Pers> operator *(DPoint<Pers> v, float a)
		{
			return new DPoint<Pers>(v.complex * a);
		}
		public static DPoint<Pers> operator /(DPoint<Pers> v, float a)
		{
			return new DPoint<Pers>(v.complex / a);
        }

        public DPoint<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform)
        {
            return new DPoint<Pers2>(dx * transform.scalex, dy * transform.scaley);
        }
        public DPoint<Pers2> Untransform<Pers2>(Transform<Pers2, Pers> transform)
        {
            return new DPoint<Pers2>(dx / transform.scalex, dy / transform.scaley);
        }

        public void AddSvg<Pers2>(XmlDocument svg, XmlNode node, string dxname, string dyname, Transform<Pers, Pers2> transform)
        {
            var v = Transform(transform);

            var dxAttribute = svg.CreateAttribute(dxname);
            dxAttribute.Value = v.dx.ToString();
            svg.AppendChild(dxAttribute);

            var dyAttribute = svg.CreateAttribute(dyname);
            dyAttribute.Value = v.dy.ToString();
            svg.AppendChild(dyAttribute);
        }

        override public string ToString()
        {
            return string.Format("{0} {1}", dx, dy);
        }
    }

    public partial struct Size<Pers>
    {
        public float value;

        public Size(float value)
        {
            this.value = value;
        }
        public static Size<Pers> operator+(Size<Pers> s, Size<Pers> t)
        {
            return new Size<Pers>(s.value + t.value);
        }
        public static Size<Pers> operator -(Size<Pers> s, Size<Pers> t)
        {
            return new Size<Pers>(s.value - t.value);
        }
        public static Size<Pers> operator *(float a, Size<Pers> s)
        {
            return new Size<Pers>(a * s.value);
        }
        public static Size<Pers> operator *(Size<Pers> s, float a)
        {
            return new Size<Pers>(s.value * a);
        }
        public static Size<Pers> operator /(Size<Pers> s, float a)
        {
            return new Size<Pers>(s.value / a);
        }

        public Size<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform)
        {
            return new Size<Pers2>(value * transform.scale);
        }
        public Size<Pers2> Untransform<Pers2>(Transform<Pers2, Pers> transform)
        {
            return new Size<Pers2>(value / transform.scale);
        }

        public void AddSvg<Pers2>(XmlDocument svg, XmlNode node, string name, Transform<Pers, Pers2> transform)
        {
            var v = Transform(transform);
            var attribute = svg.CreateAttribute(name);
            attribute.Value = v.value.ToString();
            svg.AppendChild(attribute);
        }
    }

	public partial struct Transform<PersS, PersT>
    {
        public float scale;
        public bool flipx, flipy;
        public Point<PersS> originS;
        public Point<PersT> originT;

        public float scalex => flipx ? -scale : scale;
        public float scaley => flipy ? -scale : scale;

        public Transform(float scale, bool flipx = false, bool flipy = false, Point<PersS> originS = default(Point<PersS>), Point<PersT> originT = default(Point<PersT>))
        {
            this.scale = scale; this.flipx = flipx; this.flipy = flipy; this.originS = originS; this.originT = originT;
        }
	}

	public struct Internal { }
    public struct External { }
}