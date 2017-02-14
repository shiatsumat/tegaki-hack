using System;

namespace tegaki_hack
{
    public partial struct Complex
	{
		public float re, im;
		public float norm => (float)Math.Sqrt(re * re + im * im);
		public float arg => (float)(Math.Atan2(im, re) * 180 / Math.PI); // (-180, +180]

		public Complex(float re, float im = 0)
		{
			this.re = re; this.im = im;
		}
        public static Complex Polar(float arg, float norm = 1)
        {
            float argr = arg * (float)Math.PI / 180;
            return new Complex((float)Math.Cos(argr) * norm, (float)Math.Sin(argr) * norm);
        }

		public static Complex operator +(Complex z, Complex w)
		{
			return new Complex(z.re + w.re, z.im + w.im);
		}
		public static Complex operator +(Complex z, float a)
		{
			return new Complex(z.re + a, z.im);
		}
		public static Complex operator +(float a, Complex z)
		{
			return new Complex(a + z.re, z.im);
		}
		public static Complex operator -(Complex z, Complex w)
		{
			return new Complex(z.re - w.re, z.im - w.im);
		}
		public static Complex operator -(Complex z, float a)
		{
			return new Complex(z.re - a, z.im);
		}
		public static Complex operator -(float a, Complex z)
		{
			return new Complex(a - z.re, -z.im);
		}
		public static Complex operator *(Complex z, Complex w)
		{
			return new Complex(z.re * w.re - z.im * w.im, z.re * w.im + z.im * w.re);
		}
		public static Complex operator *(Complex z, float a)
		{
			return new Complex(z.re * a, z.im * a);
		}
		public static Complex operator *(float a, Complex z)
		{
			return new Complex(a * z.re, a * z.im);
		}
		public static Complex operator /(Complex z, Complex w)
		{
			return new Complex((z.re * w.re + z.im * w.im) / (w.norm * w.norm), (-z.re * w.im + z.im * w.re) / (w.norm * w.norm));
		}
		public static Complex operator /(Complex z, float a)
		{
			return new Complex(z.re / a, z.im / a);
		}
		public static Complex operator /(float a, Complex z)
		{
			return new Complex((a * z.re) / (z.norm * z.norm), (-a * z.im) / (z.norm * z.norm));
		}
	}
}