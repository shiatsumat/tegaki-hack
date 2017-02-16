using System;
using System.Collections.Generic;
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

    public class ColorSetter : LinearLayout
    {
        public event Action ColorChanged;

        Button colorIndicator;
        NumberPicker rPicker, gPicker, bPicker, aPicker;
        SeekBar rSeekBar, gSeekBar, bSeekBar, aSeekBar, hSeekBar, sSeekBar, lSeekBar;
        TextView hText, sText, lText;
        Color _color;
        int prevh, prevs;
        bool setting;

        public Color Color
        {
            get { return _color; }
            set { _color = value; colorChanged(); }
        }
        public byte R
        {
            get { return _color.R; }
            set { _color.R = value; colorChanged(); }
        }
        public byte G
        {
            get { return _color.G; }
            set { _color.G = value; colorChanged(); }
        }
        public byte B
        {
            get { return _color.B; }
            set { _color.B = value; colorChanged(); }
        }
        public byte A
        {
            get { return _color.A; }
            set { _color.A = value; colorChanged(); }
        }
        public float H
        {
            get { return _color.H; }
            set { prevh = (int)value; _color = Color.ByHsla(value, prevs, L, A); colorChanged(); }
        }
        public float S
        {
            get { return _color.S; }
            set { prevs = (int)value; _color = Color.ByHsla(prevh, value, L, A); colorChanged(); }
        }
        public float L
        {
            get { return _color.L; }
            set { _color = Color.ByHsla(prevh, prevs, value, A); colorChanged(); }
        }

        public ColorSetter(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { Initialize(); }

        public ColorSetter(Context context, IAttributeSet attrs, int defStyleAttr) :
                base(context, attrs, defStyleAttr)
        { Initialize(); }


        void Initialize()
        {
            prevh = prevs = 0;
            setting = false;

            Inflate(Context, Resource.Layout.ColorSetter, this);

            colorIndicator = FindViewById<Button>(Resource.Id.ColorIndicator);
            colorIndicator.Click += (o, e) =>
            {
                Util.ShowToast(Context, "color");
            };

            rPicker = FindViewById<NumberPicker>(Resource.Id.RPicker);
            gPicker = FindViewById<NumberPicker>(Resource.Id.GPicker);
            bPicker = FindViewById<NumberPicker>(Resource.Id.BPicker);
            aPicker = FindViewById<NumberPicker>(Resource.Id.APicker);
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
            rPicker.ValueChanged += (o, e) => { if (!setting) R = (byte)rPicker.Value; };
            gPicker.ValueChanged += (o, e) => { if (!setting) G = (byte)gPicker.Value; };
            bPicker.ValueChanged += (o, e) => { if (!setting) B = (byte)bPicker.Value; };
            aPicker.ValueChanged += (o, e) => { if (!setting) A = (byte)aPicker.Value; };

            rSeekBar = FindViewById<SeekBar>(Resource.Id.RSeekBar);
            gSeekBar = FindViewById<SeekBar>(Resource.Id.GSeekBar);
            bSeekBar = FindViewById<SeekBar>(Resource.Id.BSeekBar);
            aSeekBar = FindViewById<SeekBar>(Resource.Id.ASeekBar);
            hSeekBar = FindViewById<SeekBar>(Resource.Id.HSeekBar);
            sSeekBar = FindViewById<SeekBar>(Resource.Id.SSeekBar);
            lSeekBar = FindViewById<SeekBar>(Resource.Id.LSeekBar);
            rSeekBar.ProgressChanged += (o, e) => { if (!setting) R = (byte)rSeekBar.Progress; };
            gSeekBar.ProgressChanged += (o, e) => { if (!setting) G = (byte)gSeekBar.Progress; };
            bSeekBar.ProgressChanged += (o, e) => { if (!setting) B = (byte)bSeekBar.Progress; };
            aSeekBar.ProgressChanged += (o, e) => { if (!setting) A = (byte)aSeekBar.Progress; };
            hSeekBar.ProgressChanged += (o, e) => { if (!setting) H = hSeekBar.Progress; };
            sSeekBar.ProgressChanged += (o, e) => { if (!setting) S = sSeekBar.Progress; };
            lSeekBar.ProgressChanged += (o, e) => { if (!setting) L = lSeekBar.Progress; };

            hText = FindViewById<TextView>(Resource.Id.HText);
            sText = FindViewById<TextView>(Resource.Id.SText);
            lText = FindViewById<TextView>(Resource.Id.LText);
        }

        void SetControls()
        {
            setting = true;

            colorIndicator.BackgroundTintList = ColorStateList.ValueOf(_color.ToNative());
            rPicker.Value = R;
            gPicker.Value = G;
            bPicker.Value = B;
            aPicker.Value = A;
            rSeekBar.Progress = R;
            gSeekBar.Progress = G;
            bSeekBar.Progress = B;
            aSeekBar.Progress = A;
            hSeekBar.Progress = float.IsNaN(H) ? prevh : (prevh = (int)Math.Round(H) % 360);
            sSeekBar.Progress = S == 0 ? prevs : (prevs = (int)Math.Round(S));
            lSeekBar.Progress = (int)Math.Round(L);

            rSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.ByHsla(0, R * 100.0f / 255.0f, 50, 255).ToNative());
            gSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.ByHsla(120, G * 100.0f / 255.0f, 50, 255).ToNative());
            bSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.ByHsla(240, B * 100.0f / 255.0f, 50, 255).ToNative());
            aSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.ByRgba(R, G, B, (byte)((A + 64.0f) * 255.0f / 319.0f)).ToNative());
            var hColors = ColorStateList.ValueOf(Color.ByHsla(prevh, 100, 50, 255).ToNative());
            hText.SetTextColor(hColors);
            hSeekBar.ThumbTintList = hColors;
            var sColors = ColorStateList.ValueOf(Color.ByHsla(prevh, prevs, 50, 255).ToNative());
            sText.SetTextColor(sColors);
            sSeekBar.ThumbTintList = sColors;
            var lColors = ColorStateList.ValueOf(Color.ByHsla(prevh, 100, L, 255).ToNative());
            lText.SetTextColor(lColors);
            lSeekBar.ThumbTintList = lColors;

            setting = false;
        }

        void colorChanged()
        {
            SetControls();
            ColorChanged?.Invoke();
        }
    }

    public class SizeSetter : LinearLayout
    {
        public event Action SizeChanged;

        NumberPicker centi;
        Spinner pers;

        public SizeEither Size
        {
            get { return new SizeEither(centi.Value / 100.0f, pers.SelectedItemPosition == 0); }
            set { centi.Value = (int)Math.Round(value.Value * 100.0f); pers.SetSelection(value.IsInternal ? 0 : 1); }
        }

        public SizeSetter(Context context, IAttributeSet attrs) :
                base(context, attrs)
        { Initialize(); }

        public SizeSetter(Context context, IAttributeSet attrs, int defStyleAttr) :
                base(context, attrs, defStyleAttr)
        { Initialize(); }

        void Initialize()
        {
            Inflate(Context, Resource.Layout.SizeSetter, this);

            centi = FindViewById<NumberPicker>(Resource.Id.Centi);
            pers = FindViewById<Spinner>(Resource.Id.Pers);
            centi.MinValue = 1;
            centi.MaxValue = 10000;
            var centstrings = new List<string>();
            for (int i = centi.MinValue; i <= centi.MaxValue; i++)
            {
                centstrings.Add(string.Format("{0:f2}", i / 100.0));
            }
            centi.SetDisplayedValues(centstrings.ToArray());
            centi.WrapSelectorWheel = false;
            centi.ValueChanged += (o, e) => SizeChanged?.Invoke();
            pers.ItemSelected += (o, e) => SizeChanged?.Invoke();
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