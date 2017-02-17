using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace tegaki_hack
{
    public partial struct Point<Pers>
    {
        public float X, Y;
        public Complex Complex => new Complex(X, Y);

        public Point(float x, float y)
        {
            X = x; Y = y;
        }
        public Point(Complex z)
        {
            X = z.Re; Y = z.Im;
        }
        public static Point<Pers> operator +(Point<Pers> p, DPoint<Pers> v)
        {
            return new Point<Pers>(p.Complex + v.Complex);
        }
        public static Point<Pers> operator +(DPoint<Pers> v, Point<Pers> p)
        {
            return new Point<Pers>(v.Complex + p.Complex);
        }
        public static Point<Pers> operator -(Point<Pers> p, DPoint<Pers> v)
        {
            return new Point<Pers>(p.Complex - v.Complex);
        }
        public static DPoint<Pers> operator -(Point<Pers> p, Point<Pers> q)
        {
            return new DPoint<Pers>(p.Complex - q.Complex);
        }
        public float DistanceTo(Point<Pers> p)
        {
            return (this - p).Norm;
        }
        public static Point<Pers> divide(Point<Pers> p, Point<Pers> q, float a, float b)
        {
            return new Point<Pers>((p.Complex * b + q.Complex * a) / (a + b));
        }
        public static Point<Pers> midpoint(Point<Pers> p, Point<Pers> q)
        {
            return divide(p, q, 1, 1);
        }

        public Point<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform)
        {
            var v = this - transform.originS;
            return transform.originT + new DPoint<Pers2>(v.Dx * transform.scalex, v.Dy * transform.scaley);
        }
        public Point<Pers2> Untransform<Pers2>(Transform<Pers2, Pers> transform)
        {
            var v = this - transform.originT;
            return transform.originS + new DPoint<Pers2>(v.Dx / transform.scalex, v.Dy / transform.scaley);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", X, Y);
        }
    }

    public partial struct DPoint<Pers>
    {
        public float Dx, Dy;
        public Complex Complex => new Complex(Dx, Dy);
        public float Norm => Complex.Norm;
        public float Arg => Complex.Arg;

        public DPoint(float dx, float dy)
        {
            Dx = dx; Dy = dy;
        }
        public DPoint(Complex z)
        {
            Dx = z.Re; Dy = z.Im;
        }
        public static DPoint<Pers> operator +(DPoint<Pers> v, DPoint<Pers> w)
        {
            return new DPoint<Pers>(v.Complex + w.Complex);
        }
        public static DPoint<Pers> operator -(DPoint<Pers> v, DPoint<Pers> w)
        {
            return new DPoint<Pers>(v.Complex - w.Complex);
        }
        public static DPoint<Pers> operator *(float a, DPoint<Pers> v)
        {
            return new DPoint<Pers>(a * v.Complex);
        }
        public static DPoint<Pers> operator *(DPoint<Pers> v, float a)
        {
            return new DPoint<Pers>(v.Complex * a);
        }
        public static DPoint<Pers> operator /(DPoint<Pers> v, float a)
        {
            return new DPoint<Pers>(v.Complex / a);
        }

        public DPoint<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform)
        {
            return new DPoint<Pers2>(Dx * transform.scalex, Dy * transform.scaley);
        }
        public DPoint<Pers2> Untransform<Pers2>(Transform<Pers2, Pers> transform)
        {
            return new DPoint<Pers2>(Dx / transform.scalex, Dy / transform.scaley);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Dx, Dy);
        }
    }

    public partial struct Size<Pers>
    {
        public float value;

        public Size(float value)
        {
            this.value = value;
        }
        public static Size<Pers> operator +(Size<Pers> s, Size<Pers> t)
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

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public partial struct Transform<PersS, PersT>
    {
        public float _scale;
        public float scale
        {
            get { return _scale; }
            set { if (!(value > 0)) throw InvalidScale(); _scale = value; }
        }
        public bool flipx, flipy;
        public Point<PersS> originS;
        public Point<PersT> originT;

        public float scalex => flipx ? -scale : scale;
        public float scaley => flipy ? -scale : scale;

        static Exception InvalidScale()
        {
            return new InvalidOperationException("Invalid Scale for Transform");
        }

        public Transform(float scale, bool flipx = false, bool flipy = false, Point<PersS> originS = default(Point<PersS>), Point<PersT> originT = default(Point<PersT>))
        {
            if (!(scale > 0)) throw InvalidScale();
            _scale = scale; this.flipx = flipx; this.flipy = flipy; this.originS = originS; this.originT = originT;
        }

        public static Transform<Pers, Pers> Identity<Pers>()
        {
            return new Transform<Pers, Pers>(1, false, false);
        }
    }

    public struct Internal { }
    public struct External { }

    public partial struct SizeEither
    {
        public float Value;
        public bool IsInternal;
        public SizeEither(float value, bool isInternal)
        {
            Value = value; IsInternal = isInternal;
        }
        public SizeEither(Size<Internal> a)
        {
            Value = a.value; IsInternal = true;
        }
        public SizeEither(Size<External> a)
        {
            Value = a.value; IsInternal = false;
        }
        public float Transform(Transform<Internal, External> transform)
        {
            if (IsInternal) return Value * transform.scale;
            else return Value;
        }
    }

    public static partial class Geometry
    {

        public static List<Point<PersT>> Transform<PersS, PersT>(this List<Point<PersS>> ps, Transform<PersS, PersT> transform)
        {
            var res = new List<Point<PersT>>();
            foreach (var p in ps)
            {
                res.Add(p.Transform(transform));
            }
            return res;
        }

        public static XElement AddSvg(this XElement element, Point<Internal> p, string xname, string yname, Transform<Internal, External> transform)
        {
            var pT = p.Transform(transform);
            element.Add(
                new XAttribute(xname, pT.X.ToString()),
                new XAttribute(yname, pT.Y.ToString()));
            return element;
        }
        public static XElement AddSvg(this XElement element, DPoint<Internal> v, string dxname, string dyname, Transform<Internal, External> transform)
        {
            var vT = v.Transform(transform);
            element.Add(
                new XAttribute(dxname, vT.Dx.ToString()),
                new XAttribute(dyname, vT.Dy.ToString()));
            return element;
        }
        public static XElement AddSvg(this XElement element, Size<Internal> a, string name, Transform<Internal, External> transform)
        {
            element.Add(
                new XAttribute(name, a.Transform(transform).ToString()));
            return element;
        }
        public static XElement AddSvg(this XElement element, SizeEither a, string name, Transform<Internal, External> transform)
        {
            element.Add(
                new XAttribute(name, a.Transform(transform).ToString()));
            return element;
        }

        static Point<Pers> InterpolateHelper<Pers>(Point<Pers> p, Point<Pers> q, Point<Pers> r)
        {
            var dpq = p.DistanceTo(q);
            var dqr = q.DistanceTo(r);
            var s = dpq + dqr;
            var t = s < Util.EPS ? 0 : 0.5f * dqr / s;
            var smooth_value = 1.0f;
            return q + smooth_value * t * (r - p);
        }
        public static BezierInfo<Pers> ToBezier<Pers>(this List<Point<Pers>> ps, bool closed)
        {
            var res = new BezierInfo<Pers>();
            res.from = ps[0];
            res.controltos = new List<ConTrolTo<Pers>>();
            for (int i = 1; i < (closed ? ps.Count + 1 : ps.Count); i++)
            {
                var p0 = closed || i >= 2 ?
                        ps.LoopGet(i - 2) :
                        ps[0];
                var p1 = ps.LoopGet(i - 1);
                var p2 = ps.LoopGet(i);
                var p3 = closed || i < ps.Count - 1 ?
                    ps.LoopGet(i + 1) :
                    ps[ps.Count - 1];
                res.controltos.Add(new ConTrolTo<Pers>(InterpolateHelper(p0, p1, p2), InterpolateHelper(p3, p2, p1), p2));
            }
            return res;
        }

    }

    public struct ConTrolTo<Pers>
    {
        public Point<Pers> Con, Trol, To;
        public ConTrolTo(Point<Pers> con, Point<Pers> trol, Point<Pers> to)
        {
            Con = con; Trol = trol; To = to;
        }
        public ConTrolTo<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform)
        {
            return new ConTrolTo<Pers2>(Con.Transform(transform), Trol.Transform(transform), To.Transform(transform));
        }
    }
    public struct BezierInfo<Pers>
    {
        public Point<Pers> from;
        public List<ConTrolTo<Pers>> controltos;

        public BezierInfo<Pers2> Transform<Pers2>(Transform<Pers, Pers2> transform)
        {
            var res = new BezierInfo<Pers2>();
            res.from = from.Transform(transform);
            res.controltos = new List<ConTrolTo<Pers2>>();
            foreach (var controlto in controltos)
            {
                res.controltos.Add(controlto.Transform(transform));
            }
            return res;
        }
    }
}