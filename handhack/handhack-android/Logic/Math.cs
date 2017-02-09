using static System.Math;

namespace handhack_android
{
    public struct Complex
	{
		public double re, im;
		public double norm => Sqrt(re * re + im * im);
		public double arg => Atan2(im, re); // (-ƒÎ, +ƒÎ]

		public Complex(double re, double im = 0)
		{
			this.re = re; this.im = im;
		}

		public static Complex operator +(Complex z, Complex w)
		{
			return new Complex(z.re + w.re, z.im + w.im);
		}
		public static Complex operator +(Complex z, double a)
		{
			return new Complex(z.re + a, z.im);
		}
		public static Complex operator +(double a, Complex z)
		{
			return new Complex(a + z.re, z.im);
		}
		public static Complex operator -(Complex z, Complex w)
		{
			return new Complex(z.re - w.re, z.im - w.im);
		}
		public static Complex operator -(Complex z, double a)
		{
			return new Complex(z.re - a, z.im);
		}
		public static Complex operator -(double a, Complex z)
		{
			return new Complex(a - z.re, -z.im);
		}
		public static Complex operator *(Complex z, Complex w)
		{
			return new Complex(z.re * w.re - z.im * w.im, z.re * w.im + z.im * w.re);
		}
		public static Complex operator *(Complex z, double a)
		{
			return new Complex(z.re * a, z.im * a);
		}
		public static Complex operator *(double a, Complex z)
		{
			return new Complex(a * z.re, a * z.im);
		}
		public static Complex operator /(Complex z, Complex w)
		{
			return new Complex((z.re * w.re + z.im * w.im) / (w.norm * w.norm), (-z.re * w.im + z.im * w.re) / (w.norm * w.norm));
		}
		public static Complex operator /(Complex z, double a)
		{
			return new Complex(z.re / a, z.im / a);
		}
		public static Complex operator /(double a, Complex z)
		{
			return new Complex((a * z.re) / (z.norm * z.norm), (-a * z.im) / (z.norm * z.norm));
		}
	}
}