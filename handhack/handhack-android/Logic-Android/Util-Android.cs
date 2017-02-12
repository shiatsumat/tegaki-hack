using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using NativeColor = Android.Graphics.Color;

namespace handhack
{
    public delegate void CanvasDelegate(Canvas canvas);

    public class ExtensibleView : View
    {
        public CanvasDelegate Drawing;

        public ExtensibleView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { }

        public ExtensibleView(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        { }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            Drawing(canvas);
        }
    }

    public class ColorPicker : LinearLayout
    {
        public NumberPicker r, g, b, a;
        public Color color
        {
            get { return new Color((byte)r.Value, (byte)g.Value, (byte)b.Value, (byte)a.Value); }
            set { r.Value = value.r; g.Value = value.g; b.Value = value.b; a.Value = value.a; }
        }

        public ColorPicker(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { Initialize(); }

        public ColorPicker(Context context, IAttributeSet attrs, int defStyleAttr) :
                base(context, attrs, defStyleAttr)
        { Initialize(); }


        void Initialize()
        {
            Inflate(Context, Resource.Layout.ColorPicker, this);
            r = FindViewById<NumberPicker>(Resource.Id.R);
            b = FindViewById<NumberPicker>(Resource.Id.B);
            g = FindViewById<NumberPicker>(Resource.Id.G);
            a = FindViewById<NumberPicker>(Resource.Id.A);
            r.MinValue = b.MinValue = g.MinValue = a.MinValue = 0;
            r.MaxValue = b.MaxValue = g.MaxValue = a.MaxValue = 255;
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