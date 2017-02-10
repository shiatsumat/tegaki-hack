using System;
using System.Xml.Linq;

namespace handhack
{
    public partial struct Point<Pers> where Pers : IPers
	{
		public float _x, _y;
        public float x
        {
            get { pers.MakeSureNoninternal(); return _x; }
            set { pers.MakeSureNoninternal();_x = value; }
        }
        public float y
        {
            get { pers.MakeSureNoninternal(); return _y; }
            set { pers.MakeSureNoninternal(); _y = value; }
        }
        Pers pers;
		public Complex complex => new Complex(_x, _y);
        public Size<Pers> X { get { return new Size<Pers>(_x); } set { _x = value._value; } }
        public Size<Pers> Y { get { return new Size<Pers>(_y); } set { _y = value._value; } }

        public Point(float x, float y)
		{
			this._x = x; this._y = y; pers = default(Pers);
		}
		public Point(Complex z)
		{
            _x = z.re; _y = z.im; pers = default(Pers);
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
		public float distance(Point<Pers> p)
		{
			return (this - p).norm;
		}
		public static Point<Pers> divide(Point<Pers> p, Point<Pers> q, float a, float b)
		{
			return new Point<Pers>((p.complex * b + q.complex * a) / (a + b));
		}
		public static Point<Pers> midpoint(Point<Pers> p, Point<Pers> q)
		{
			return divide(p, q, 1, 1);
		}

		public Point<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform) where Pers2 : IPers
		{
            var v = this - transform.originS;
			return transform.originT + new DPoint<Pers2>(v._dx * transform.scalex, v._dy * transform.scaley);
		}
		public Point<Pers2> Untransform<Pers2>(Transform<Pers2, Pers> transform) where Pers2 : IPers
        {
			var v = this - transform.originT;
			return transform.originS + new DPoint<Pers2>(v._dx / transform.scalex, v._dy / transform.scaley);
        }

        override public string ToString()
        {
            pers.MakeSureNoninternal();
            return string.Format("{0} {1}", _x, _y);
        }
    }

	public partial struct DPoint<Pers> where Pers : IPers
	{
		public float _dx, _dy;
        public float dx
        {
            get { pers.MakeSureNoninternal(); return _dx; }
            set { pers.MakeSureNoninternal(); _dx = value; }
        }
        public float dy
        {
            get { pers.MakeSureNoninternal(); return _dy; }
            set { pers.MakeSureNoninternal(); _dy = value; }
        }
        Pers pers;
        public Complex complex => new Complex(_dx, _dy);
		public float norm => complex.norm;
        public float arg => complex.arg;
        public Size<Pers> DX { get { return new Size<Pers>(_dx); } set { _dx = value._value; } }
        public Size<Pers> DY { get { return new Size<Pers>(_dy); } set { _dy = value._value; } }

        public DPoint(float dx, float dy)
		{
			this._dx = dx; this._dy = dy; pers = default(Pers);
        }
		public DPoint(Complex z)
		{
            _dx = z.re; _dy = z.im; pers = default(Pers);
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

        public DPoint<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform) where Pers2 : IPers
        {
            return new DPoint<Pers2>(_dx * transform.scalex, _dy * transform.scaley);
        }
        public DPoint<Pers2> Untransform<Pers2>(Transform<Pers2, Pers> transform) where Pers2 : IPers
        {
            return new DPoint<Pers2>(_dx / transform.scalex, _dy / transform.scaley);
        }

        override public string ToString()
        {
            pers.MakeSureNoninternal();
            return string.Format("{0} {1}", _dx, _dy);
        }
    }

    public partial struct Size<Pers> where Pers : IPers
    {
        public float _value;
        public float value
        {
            get { pers.MakeSureNoninternal(); return _value; }
            set { pers.MakeSureNoninternal(); _value = value; }
        }
        Pers pers;

        public Size(float value)
        {
            this._value = value; pers = default(Pers);
        }
        public static Size<Pers> operator+(Size<Pers> s, Size<Pers> t)
        {
            return new Size<Pers>(s._value + t._value);
        }
        public static Size<Pers> operator -(Size<Pers> s, Size<Pers> t)
        {
            return new Size<Pers>(s._value - t._value);
        }
        public static Size<Pers> operator *(float a, Size<Pers> s)
        {
            return new Size<Pers>(a * s._value);
        }
        public static Size<Pers> operator *(Size<Pers> s, float a)
        {
            return new Size<Pers>(s._value * a);
        }
        public static Size<Pers> operator /(Size<Pers> s, float a)
        {
            return new Size<Pers>(s._value / a);
        }

        public Size<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform) where Pers2 : IPers
        {
            return new Size<Pers2>(_value * transform.scale);
        }
        public Size<Pers2> Untransform<Pers2>(Transform<Pers2, Pers> transform) where Pers2 : IPers
        {
            return new Size<Pers2>(_value / transform.scale);
        }

        override public string ToString()
        {
            pers.MakeSureNoninternal();
            return _value.ToString();
        }
    }

	public partial struct Transform<PersS, PersT> where PersS : IPers where PersT : IPers
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

        public static Transform<Pers, Pers> Identity<Pers>() where Pers : IPers
        {
            return new Transform<Pers, Pers>(1, false, false);
        }
	}

    public interface IPers
    {
        void MakeSureNoninternal();
    }
    public class Internal : IPers
    {
        public void MakeSureNoninternal()
        {
            throw new InvalidProgramException("MakeSureNoninternal faied: The Pers is Internal!");
        }
    }
    public class External : IPers
    {
        public void MakeSureNoninternal() { }
    }

    public static partial class GeometryStatic
    {
        public static XElement AddSvg<X>(this XElement element, Point<Internal> p, string xname, string yname, Transform<Internal, X> transform) where X : External
        {
            var pT = p.Transform(transform);
            element.Add(
                new XAttribute(xname, pT.x.ToString()),
                new XAttribute(yname, pT.y.ToString()));
            return element;
        }
        public static XElement AddSvg<X>(this XElement element, DPoint<Internal> v, string dxname, string dyname, Transform<Internal, X> transform) where X : External
        {
            var vT = v.Transform(transform);
            element.Add(
                new XAttribute(dxname, vT.dx.ToString()),
                new XAttribute(dyname, vT.dy.ToString()));
            return element;
        }
        public static XElement AddSvg<X>(this XElement element, Size<Internal> a, string name, Transform<Internal, X> transform) where X : External
        {
            element.Add(
                new XAttribute(name, a.Transform(transform).ToString()));
            return element;
        }

        static Point<Pers> Control<Pers>(Point<Pers> p, Point<Pers> q, Point<Pers> r) where Pers : IPers
        {
            var dpq = p.distance(q);
            var dqr = q.distance(r);
            var s = dpq + dqr;
            var t = s < 1e-6 ? 0 : 0.5f * dqr / s;
            var smooth_value = 1.0f;
            return q + smooth_value * t * (r - p);
        }
        public static Point<Pers> InterpolateCon<Pers>(Point<Pers> p0, Point<Pers> p1, Point<Pers> p2, Point<Pers> p3) where Pers : IPers
        {
            return Control(p0, p1, p2);
        }
        public static Point<Pers> InterpolateTrol<Pers>(Point<Pers> p0, Point<Pers> p1, Point<Pers> p2, Point<Pers> p3) where Pers : IPers
        {
            return Control(p3, p2, p1);
        }
    }
}