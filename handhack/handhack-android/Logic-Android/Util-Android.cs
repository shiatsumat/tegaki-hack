using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using NativeColor = Android.Graphics.Color;

namespace handhack
{
    public delegate void CanvasDelegate(Canvas canvas);

	public class ExtensibleView : View
	{
        public CanvasDelegate Drawing;

        public ExtensibleView(Context context, IAttributeSet attrs) :
			base(context, attrs) {}

		public ExtensibleView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle) {}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
            Drawing(canvas);
		}
	}

    public static partial class Util
    {
        public static void Activate(this View view, bool activate)
        {
            view.BackgroundTintList = !activate ? null : ColorStateList.ValueOf(NativeColor.Orange);
        }
    }
}