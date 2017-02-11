using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace handhack
{
    public delegate void CanvasDelegate(Canvas canvas);

	public class DrawableView : View
	{
        public CanvasDelegate Drawing;

        public DrawableView(Context context, IAttributeSet attrs) :
			base(context, attrs) {}

		public DrawableView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle) {}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
            Drawing(canvas);
		}
	}
}