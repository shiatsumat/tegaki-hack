using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using NativeColor = Android.Graphics.Color;
using static System.Math;
using static tegaki_hack.UtilStatic;
using static tegaki_hack.Color;

namespace tegaki_hack
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

    public class CustomNumberPicker : NumberPicker
    {
        public event EventHandler ValueChangedByUser;
        public int AutoValue { set { if (Value != value) { Value = value; autoChanged = true; } } }

        bool autoChanged;

        public CustomNumberPicker(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { Initialize(); }

        public CustomNumberPicker(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        { Initialize(); }

        void Initialize()
        {
            autoChanged = false;
            ValueChanged += (o, e) =>
            {
                if (!autoChanged) ValueChangedByUser(o, e);
                else autoChanged = false;
            };
        }
    }

    public class CustomSeekBar : SeekBar
    {
        public event EventHandler ProgressChangedByUser;
        public int AutoProgress { set { if (Progress != value) { Progress = value; autoChanged = true; } } }

        bool autoChanged;

        public CustomSeekBar(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { Initialize(); }

        public CustomSeekBar(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        { Initialize(); }

        void Initialize()
        {
            autoChanged = false;
            ProgressChanged += (o, e) =>
            {
                if (!autoChanged) ProgressChangedByUser(o, e);
                else autoChanged = false;
            };
        }
    }

    public class ColorSetter : LinearLayout
    {
        public Button colorIndicator;
        public CustomNumberPicker rPicker, gPicker, bPicker, aPicker;
        public CustomSeekBar rSeekBar, gSeekBar, bSeekBar, aSeekBar, hSeekBar, sSeekBar, lSeekBar;
        public TextView hText, sText, lText;
        Color _color;
        int prevh, prevs;

        public Color color
        {
            get { return _color; }
            set { _color = value; SetControls(); }
        }
        public byte r
        {
            get { return _color.r; }
            set { _color.r = value; SetControls(); }
        }
        public byte g
        {
            get { return _color.g; }
            set { _color.g = value; SetControls(); }
        }
        public byte b
        {
            get { return _color.b; }
            set { _color.b = value; SetControls(); }
        }
        public byte a
        {
            get { return _color.a; }
            set { _color.a = value; SetControls(); }
        }
        public float h
        {
            get { return _color.h; }
            set { _color.h = value; SetControls(); }
        }
        public float s
        {
            get { return _color.s; }
            set { _color.s = value; SetControls(); }
        }
        public float l
        {
            get { return _color.l; }
            set { _color.l = value; SetControls(); }
        }

        public ColorSetter(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { Initialize(); }

        public ColorSetter(Context context, IAttributeSet attrs, int defStyleAttr) :
                base(context, attrs, defStyleAttr)
        { Initialize(); }


        void Initialize()
        {
            Inflate(Context, Resource.Layout.ColorSetter, this);
            colorIndicator = FindViewById<Button>(Resource.Id.ColorIndicator);
            rPicker = FindViewById<CustomNumberPicker>(Resource.Id.RPicker);
            gPicker = FindViewById<CustomNumberPicker>(Resource.Id.GPicker);
            bPicker = FindViewById<CustomNumberPicker>(Resource.Id.BPicker);
            aPicker = FindViewById<CustomNumberPicker>(Resource.Id.APicker);
            rSeekBar = FindViewById<CustomSeekBar>(Resource.Id.RSeekBar);
            gSeekBar = FindViewById<CustomSeekBar>(Resource.Id.GSeekBar);
            bSeekBar = FindViewById<CustomSeekBar>(Resource.Id.BSeekBar);
            aSeekBar = FindViewById<CustomSeekBar>(Resource.Id.ASeekBar);
            hSeekBar = FindViewById<CustomSeekBar>(Resource.Id.HSeekBar);
            sSeekBar = FindViewById<CustomSeekBar>(Resource.Id.SSeekBar);
            lSeekBar = FindViewById<CustomSeekBar>(Resource.Id.LSeekBar);
            hText = FindViewById<TextView>(Resource.Id.HText);
            sText = FindViewById<TextView>(Resource.Id.SText);
            lText = FindViewById<TextView>(Resource.Id.LText);

            rPicker.MinValue = gPicker.MinValue = bPicker.MinValue = aPicker.MinValue = 0;
            rPicker.MaxValue = gPicker.MaxValue = bPicker.MaxValue = aPicker.MaxValue = 255;
            var ffstrings = new List<string>();
            for (int i = 0; i < 256; i++)
            {
                ffstrings.Add(string.Format("{0:X2}", i));
            }
            var affstrings = ffstrings.ToArray();
            rPicker.SetDisplayedValues(affstrings);
            gPicker.SetDisplayedValues(affstrings);
            bPicker.SetDisplayedValues(affstrings);
            aPicker.SetDisplayedValues(affstrings);

            colorIndicator.Click += (o, e) =>
            {
                ShowToast(Context, "color");
            };

            rPicker.ValueChangedByUser += (o, e) => r = (byte)rPicker.Value;
            bPicker.ValueChangedByUser += (o, e) => g = (byte)gPicker.Value;
            gPicker.ValueChangedByUser += (o, e) => b = (byte)bPicker.Value;
            aPicker.ValueChangedByUser += (o, e) => a = (byte)aPicker.Value;
            rSeekBar.ProgressChangedByUser += (o, e) => r = (byte)rSeekBar.Progress;
            gSeekBar.ProgressChangedByUser += (o, e) => g = (byte)gSeekBar.Progress;
            bSeekBar.ProgressChangedByUser += (o, e) => b = (byte)bSeekBar.Progress;
            aSeekBar.ProgressChangedByUser += (o, e) => a = (byte)aSeekBar.Progress;
            hSeekBar.ProgressChangedByUser += (o, e) => { prevh = hSeekBar.Progress; color = Hsla(prevh, prevs, l, a); };
            sSeekBar.ProgressChangedByUser += (o, e) => { prevs = sSeekBar.Progress; color = Hsla(prevh, prevs, l, a); };
            lSeekBar.ProgressChangedByUser += (o, e) => { l = lSeekBar.Progress; color = Hsla(prevh, prevs, l, a); };

            prevh = prevs = 0;
        }

        void SetControls()
        {
            colorIndicator.BackgroundTintList = ColorStateList.ValueOf(_color.native);
            rPicker.AutoValue = r;
            gPicker.AutoValue = g;
            bPicker.AutoValue = b;
            aPicker.AutoValue = a;
            rSeekBar.AutoProgress = r;
            gSeekBar.AutoProgress = g;
            bSeekBar.AutoProgress = b;
            aSeekBar.AutoProgress = a;
            hSeekBar.AutoProgress = float.IsNaN(h) ? prevh : (prevh = (int)Round(h) % 360);
            sSeekBar.AutoProgress = s == 0 ? prevs : (prevs = (int)Round(s));
            lSeekBar.AutoProgress = (int)Round(l);

            rSeekBar.ThumbTintList = ColorStateList.ValueOf(Hsla(0, r * 100.0f / 255.0f, 50, 255).native);
            gSeekBar.ThumbTintList = ColorStateList.ValueOf(Hsla(120, g * 100.0f / 255.0f, 50, 255).native);
            bSeekBar.ThumbTintList = ColorStateList.ValueOf(Hsla(240, b * 100.0f / 255.0f, 50, 255).native);
            aSeekBar.ThumbTintList = ColorStateList.ValueOf(Rgba(r, g, b, (byte)((a + 64.0f) * 255.0f / 319.0f)).native);
            var hColors = ColorStateList.ValueOf(Hsla(prevh, 100, 50, 255).native);
            hText.SetTextColor(hColors);
            hSeekBar.ThumbTintList = hColors;
            var sColors = ColorStateList.ValueOf(Hsla(prevh, prevs, 50, 255).native);
            sText.SetTextColor(sColors);
            sSeekBar.ThumbTintList = sColors;
            var lColors = ColorStateList.ValueOf(Hsla(prevh, 100, l, 255).native);
            lText.SetTextColor(lColors);
            lSeekBar.ThumbTintList = lColors;
        }
    }

    public static partial class UtilStatic
    {
        public static void Activate(this View view, bool activate)
        {
            view.BackgroundTintList = !activate ? null : ColorStateList.ValueOf(NativeColor.Orange);
        }
        public static void ShowToast(Context context, string text, ToastLength duration = ToastLength.Short)
        {
            Toast.MakeText(context, text, duration).Show();
        }
    }
}