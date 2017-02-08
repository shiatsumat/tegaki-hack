using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace handhack_android
{
	public class Editcanvas : View
	{
		public Editcanvas(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public Editcanvas(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		private void Initialize()
		{
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			var paint = new Paint();
			paint.Color = Color.Black;
			paint.TextSize = 100;
			canvas.DrawText(String.Format("{0}x{1}", canvas.Width, canvas.Height), 100, 100, paint);
		}
	}
}