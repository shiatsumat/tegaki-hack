using System;
using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using Android.Util;
using Android.Content.Res;

namespace tegaki_hack
{
    public class ColorSetter : LinearLayout
    {
        Button colorIndicator;
        NumberPicker rNumberPicker, gNumberPicker, bNumberPicker, aNumberPicker;
        SeekBar rSeekBar, gSeekBar, bSeekBar, aSeekBar, hSeekBar, sSeekBar, lSeekBar;
        TextView hText, sText, lText;
        Color _color;
        int prevh, prevs;
        bool setting;
        public event Action ColorChanged;

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
            InitializeVariables();
            InitializeView();
            InitializeColorIndicator();
            InitializeRgbaNumberPickers();
            InitializeRgbaSeekBars();
            InitializeHslSeekBars();
        }
        void InitializeVariables()
        {
            prevh = prevs = 0;
            setting = false;
        }
        void InitializeView()
        {
            Inflate(Context, Resource.Layout.ColorSetter, this);
        }
        void InitializeColorIndicator()
        {
            colorIndicator = FindViewById<Button>(Resource.Id.ColorIndicator);
            colorIndicator.Click += (o, e) =>
            {
                Util.ShowToast(Context, "color");
            };
        }
        void InitializeRgbaNumberPickers()
        {
            rNumberPicker = FindViewById<NumberPicker>(Resource.Id.RPicker);
            gNumberPicker = FindViewById<NumberPicker>(Resource.Id.GPicker);
            bNumberPicker = FindViewById<NumberPicker>(Resource.Id.BPicker);
            aNumberPicker = FindViewById<NumberPicker>(Resource.Id.APicker);
            rNumberPicker.MinValue = gNumberPicker.MinValue = bNumberPicker.MinValue = aNumberPicker.MinValue = 0;
            rNumberPicker.MaxValue = gNumberPicker.MaxValue = bNumberPicker.MaxValue = aNumberPicker.MaxValue = 255;
            var ffstrings = new List<string>();
            for (int i = 0; i < 256; i++)
            {
                ffstrings.Add(string.Format("{0:X2}", i));
            }
            var affstrings = ffstrings.ToArray();
            rNumberPicker.SetDisplayedValues(affstrings);
            gNumberPicker.SetDisplayedValues(affstrings);
            bNumberPicker.SetDisplayedValues(affstrings);
            aNumberPicker.SetDisplayedValues(affstrings);
            rNumberPicker.ValueChanged += (o, e) => { if (!setting) R = (byte)rNumberPicker.Value; };
            gNumberPicker.ValueChanged += (o, e) => { if (!setting) G = (byte)gNumberPicker.Value; };
            bNumberPicker.ValueChanged += (o, e) => { if (!setting) B = (byte)bNumberPicker.Value; };
            aNumberPicker.ValueChanged += (o, e) => { if (!setting) A = (byte)aNumberPicker.Value; };
        }
        void InitializeRgbaSeekBars()
        {
            rSeekBar = FindViewById<SeekBar>(Resource.Id.RSeekBar);
            gSeekBar = FindViewById<SeekBar>(Resource.Id.GSeekBar);
            bSeekBar = FindViewById<SeekBar>(Resource.Id.BSeekBar);
            aSeekBar = FindViewById<SeekBar>(Resource.Id.ASeekBar);
            rSeekBar.ProgressChanged += (o, e) => { if (!setting) R = (byte)rSeekBar.Progress; };
            gSeekBar.ProgressChanged += (o, e) => { if (!setting) G = (byte)gSeekBar.Progress; };
            bSeekBar.ProgressChanged += (o, e) => { if (!setting) B = (byte)bSeekBar.Progress; };
            aSeekBar.ProgressChanged += (o, e) => { if (!setting) A = (byte)aSeekBar.Progress; };
        }
        void InitializeHslSeekBars()
        {
            hSeekBar = FindViewById<SeekBar>(Resource.Id.HSeekBar);
            sSeekBar = FindViewById<SeekBar>(Resource.Id.SSeekBar);
            lSeekBar = FindViewById<SeekBar>(Resource.Id.LSeekBar);
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
            SetColorIndicator();
            SetRgbaNumberPickers();
            SetRgbaSeekBars();
            SetHslSeekBars();
            setting = false;
        }
        void SetColorIndicator()
        {
            colorIndicator.BackgroundTintList = ColorStateList.ValueOf(_color.ToNative());
        }
        void SetRgbaNumberPickers()
        {
            rNumberPicker.Value = R;
            gNumberPicker.Value = G;
            bNumberPicker.Value = B;
            aNumberPicker.Value = A;
        }
        void SetRgbaSeekBars()
        {
            rSeekBar.Progress = R;
            gSeekBar.Progress = G;
            bSeekBar.Progress = B;
            aSeekBar.Progress = A;
            rSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.ByHsla(0, R * 100.0f / 255.0f, 50, 255).ToNative());
            gSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.ByHsla(120, G * 100.0f / 255.0f, 50, 255).ToNative());
            bSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.ByHsla(240, B * 100.0f / 255.0f, 50, 255).ToNative());
            aSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.ByRgba(R, G, B, (byte)((A + 64.0f) * 255.0f / 319.0f)).ToNative());
        }
        void SetHslSeekBars()
        {
            hSeekBar.Progress = float.IsNaN(H) ? prevh : (prevh = (int)Math.Round(H) % 360);
            sSeekBar.Progress = S == 0 ? prevs : (prevs = (int)Math.Round(S));
            lSeekBar.Progress = (int)Math.Round(L);
            var hColors = ColorStateList.ValueOf(Color.ByHsla(prevh, 100, 50, 255).ToNative());
            hText.SetTextColor(hColors);
            hSeekBar.ThumbTintList = hColors;
            var sColors = ColorStateList.ValueOf(Color.ByHsla(prevh, prevs, 50, 255).ToNative());
            sText.SetTextColor(sColors);
            sSeekBar.ThumbTintList = sColors;
            var lColors = ColorStateList.ValueOf(Color.ByHsla(prevh, 100, L, 255).ToNative());
            lText.SetTextColor(lColors);
            lSeekBar.ThumbTintList = lColors;
        }

        void colorChanged()
        {
            SetControls();
            ColorChanged?.Invoke();
        }
    }
}