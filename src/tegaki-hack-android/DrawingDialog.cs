using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.App;
using Android.Widget;
using Android.Views;

namespace tegaki_hack
{
    public class DrawingDialog
    {
        View view;
        AlertDialog dialog;
        Drawing drawing;

        Activity activity;
        Action<Drawing> ok;

        ExtensibleView colorSample;
        ColorSetter lineColor, fillColor;
        SizeSetter lineWidth;

        ExtensibleView lineCapJoinSample;
        Spinner lineCap, lineJoin;
        ExtensibleView miterLimitSample;
        NumberPicker miterLimitDeci;

        ExtensibleView fillRuleSample;
        Spinner fillRule;

        public static Task<DrawingDialog> AsyncNew(Activity activity, Action<Drawing> ok)
        {
            return Task.Run<DrawingDialog>(() => new DrawingDialog(activity, ok));
        }
        public DrawingDialog(Activity activity, Action<Drawing> ok)
        {
            this.activity = activity; this.ok = ok;
            view = activity.LayoutInflater.Inflate(Resource.Layout.DrawingDialog, null);
        }

        public void Show(Drawing drawing)
        {
            if (dialog == null) Initialize();
            SetDrawing(drawing);
            dialog.Show();
        }

        void SetDrawing(Drawing drawing)
        {
            this.drawing = new Drawing(drawing);
            lineColor.Color = drawing.LineColor;
            fillColor.Color = drawing.FillColor;
            lineWidth.Size = drawing.LineWidth;
            lineCap.SetSelection((int)drawing.LineCap);
            lineJoin.SetSelection((int)drawing.LineJoin);
            miterLimitDeci.Value = (int)Math.Round(drawing.MiterLimit * 10.0f);
            fillRule.SetSelection((int)drawing.FillRule);
        }

        void Initialize()
        {
            InitializeColors();
            InitializeLineWidth();
            InitializeLineCapJoin();
            InitializeFillRule();
            InitializeDialog(activity, ok);
        }
        void InitializeColors()
        {
            InitializeColorSample();
            InitializeLineColor();
            InitializeFillColor();
        }
        void InitializeColorSample()
        {
            colorSample = view.FindViewById<ExtensibleView>(Resource.Id.ColorSample);
            colorSample.Draw += (canvas) =>
            {
                drawing.ColorSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 100.0f));
            };
        }
        void InitializeLineColor()
        {
            lineColor = view.FindViewById<ColorSetter>(Resource.Id.LineColor);
            lineColor.ColorChanged += () =>
            {
                drawing.LineColor = lineColor.Color;
                colorSample.Invalidate();
            };
        }
        void InitializeFillColor()
        {
            fillColor = view.FindViewById<ColorSetter>(Resource.Id.FillColor);
            fillColor.ColorChanged += () =>
            {
                drawing.FillColor = fillColor.Color;
                colorSample.Invalidate();
            };
        }
        void InitializeLineWidth()
        {
            lineWidth = view.FindViewById<SizeSetter>(Resource.Id.LineWidth);
            lineWidth.SizeChanged += () =>
            {
                drawing.LineWidth = lineWidth.Size;
            };
        }
        void InitializeLineCapJoin()
        {
            InitializeLineCapJoinSample();
            InitializeLineCap();
            InitializeLineJoin();
            InitializeMiterLimit();
        }
        void InitializeLineCapJoinSample()
        {
            lineCapJoinSample = view.FindViewById<ExtensibleView>(Resource.Id.LineCapJoinSample);
            lineCapJoinSample.Draw += (canvas) =>
            {
                drawing.LineCapJoinSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 150.0f));
            };
        }
        void InitializeLineCap()
        {
            lineCap = view.FindViewById<Spinner>(Resource.Id.LineCap);
            lineCap.ItemSelected += (o, e) =>
            {
                drawing.LineCap = (LineCap)lineCap.SelectedItemPosition;
                lineCapJoinSample.Invalidate();
            };
        }
        void InitializeLineJoin()
        {
            lineJoin = view.FindViewById<Spinner>(Resource.Id.LineJoin);
            lineJoin.ItemSelected += (o, e) =>
            {
                drawing.LineJoin = (LineJoin)lineJoin.SelectedItemPosition;
                lineCapJoinSample.Invalidate();
            };
        }
        void InitializeMiterLimit()
        {
            InitializeMiterLimitSample();
            InitializeMiterLimitDeci();
        }
        void InitializeMiterLimitSample()
        {
            miterLimitSample = view.FindViewById<ExtensibleView>(Resource.Id.MiterLimitSample);
            miterLimitSample.Draw += (canvas) =>
            {
                drawing.MiterLimitSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 150.0f));
            };
        }
        void InitializeMiterLimitDeci()
        {
            miterLimitDeci = view.FindViewById<NumberPicker>(Resource.Id.MiterLimitDeci);
            miterLimitDeci.MinValue = 0;
            miterLimitDeci.MaxValue = 1000;
            var deciStrings = new List<string>();
            for (int i = miterLimitDeci.MinValue; i <= miterLimitDeci.MaxValue; i++)
            {
                deciStrings.Add(string.Format("{0:f1}", i / 10.0));
            }
            miterLimitDeci.SetDisplayedValues(deciStrings.ToArray());
            miterLimitDeci.WrapSelectorWheel = false;
            miterLimitDeci.ValueChanged += (o, e) =>
            {
                drawing.MiterLimit = miterLimitDeci.Value / 10.0f;
                miterLimitSample.Invalidate();
            };
        }
        void InitializeFillRule()
        {
            fillRuleSample = view.FindViewById<ExtensibleView>(Resource.Id.FillRuleSample);
            fillRule = view.FindViewById<Spinner>(Resource.Id.FillRule);
            fillRuleSample.Draw += (canvas) =>
            {
                drawing.FillRuleSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 150.0f));
            };
            fillRule.ItemSelected += (o, e) =>
            {
                drawing.FillRule = (FillRule)fillRule.SelectedItemPosition;
                fillRuleSample.Invalidate();
            };
        }
        void InitializeDialog(Activity activity, Action<Drawing> ok)
        {
            dialog = Util.CreateDialog(activity, Resource.String.DrawingOptions, view,
                () => ok?.Invoke(drawing), null);
        }
    }
}