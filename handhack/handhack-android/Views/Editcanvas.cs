using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace handhack_android
{
    public delegate void OnDrawDelegate(Canvas canvas);
	public class Editcanvas : View
	{
        public OnDrawDelegate onDraw;

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

            onDraw(canvas);
		}
	}
}