using System;

namespace tegaki_hack
{
    public partial struct Complex
	{
		public float Re, Im;
		public float Norm => (float)Math.Sqrt(Re * Re + Im * Im);
		public float Arg => (float)(Math.Atan2(Im, Re) * 180 / Math.PI); // (-180, +180]

		public Complex(float re, float im = 0)
		{
            Re = re; Im = im;
		}
        public static Complex Polar(float arg, float norm = 1)
        {
            float argr = arg * (float)Math.PI / 180;
            return new Complex((float)Math.Cos(argr) * norm, (float)Math.Sin(argr) * norm);
        }

		public static Complex operator +(Complex z, Complex w)
		{
			return new Complex(z.Re + w.Re, z.Im + w.Im);
		}
		public static Complex operator +(Complex z, float a)
		{
			return new Complex(z.Re + a, z.Im);
		}
		public static Complex operator +(float a, Complex z)
		{
			return new Complex(a + z.Re, z.Im);
		}
		public static Complex operator -(Complex z, Complex w)
		{
			return new Complex(z.Re - w.Re, z.Im - w.Im);
		}
		public static Complex operator -(Complex z, float a)
		{
			return new Complex(z.Re - a, z.Im);
		}
		public static Complex operator -(float a, Complex z)
		{
			return new Complex(a - z.Re, -z.Im);
		}
		public static Complex operator *(Complex z, Complex w)
		{
			return new Complex(z.Re * w.Re - z.Im * w.Im, z.Re * w.Im + z.Im * w.Re);
		}
		public static Complex operator *(Complex z, float a)
		{
			return new Complex(z.Re * a, z.Im * a);
		}
		public static Complex operator *(float a, Complex z)
		{
			return new Complex(a * z.Re, a * z.Im);
		}
		public static Complex operator /(Complex z, Complex w)
		{
			return new Complex((z.Re * w.Re + z.Im * w.Im) / (w.Norm * w.Norm), (-z.Re * w.Im + z.Im * w.Re) / (w.Norm * w.Norm));
		}
		public static Complex operator /(Complex z, float a)
		{
			return new Complex(z.Re / a, z.Im / a);
		}
		public static Complex operator /(float a, Complex z)
		{
			return new Complex((a * z.Re) / (z.Norm * z.Norm), (-a * z.Im) / (z.Norm * z.Norm));
		}
	}
}