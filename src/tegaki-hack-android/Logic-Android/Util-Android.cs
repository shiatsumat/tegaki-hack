using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using NativeColor = Android.Graphics.Color;
using Android.App;

namespace tegaki_hack
{
    public delegate void CanvasDelegate(Canvas canvas);

    public class ExtensibleView : View
    {
        public event CanvasDelegate Drawing;

        public ExtensibleView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { }
        public ExtensibleView(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        { }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            Drawing?.Invoke(canvas);
        }
    }

    public static partial class Util
    {
        public static void Activate(this View view, bool activate)
        {
            view.BackgroundTintList = !activate ? null : ColorStateList.ValueOf(NativeColor.Orange);
        }

        public static void ShowToast(Context context, string text, ToastLength duration = ToastLength.Short)
        {
            Toast.MakeText(context, text, duration).Show();
        }

        public static AlertDialog CreateDialog(Context context, int titleId, View view, Action ok, Action cancel)
        {
            return new AlertDialog.Builder(context)
                .SetTitle(titleId)
                .SetView(view)
                .SetPositiveButton(context.GetString(Android.Resource.String.Ok), (a, s) => ok?.Invoke())
                .SetNegativeButton(context.GetString(Android.Resource.String.Cancel), (a, s) => cancel?.Invoke())
                .Create();
        }
    }
}