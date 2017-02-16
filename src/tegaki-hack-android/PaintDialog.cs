using System;
using System.Collections.Generic;
using Android.App;
using Android.Widget;

namespace tegaki_hack
{
    public class PaintDialog
    {
        AlertDialog dialog;

        Paint paint;

        ExtensibleView colorSample;
        ColorSetter strokeColor, fillColor;
        SizeSetter strokeWidth;

        ExtensibleView lineCapJoinSample;
        Spinner lineCap, lineJoin;
        NumberPicker miterLimitDeci;

        ExtensibleView fillRuleSample;
        Spinner fillRule;

        public PaintDialog(Activity activity, Action<Paint> ok)
        {
            var view = activity.LayoutInflater.Inflate(Resource.Layout.PaintDialog, null);

            colorSample = view.FindViewById<ExtensibleView>(Resource.Id.ColorSample);
            colorSample.Drawing += (canvas) =>
            {
                paint.ColorSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 100.0f));
            };
            strokeColor = view.FindViewById<ColorSetter>(Resource.Id.StrokeColor);
            strokeColor.ColorChanged += () =>
            {
                paint.StrokeColor = strokeColor.Color;
                colorSample.Invalidate();
            };
            fillColor = view.FindViewById<ColorSetter>(Resource.Id.FillColor);
            fillColor.ColorChanged += () =>
            {
                paint.FillColor = fillColor.Color;
                colorSample.Invalidate();
            };

            strokeWidth = view.FindViewById<SizeSetter>(Resource.Id.StrokeWidth);
            strokeWidth.SizeChanged += () =>
            {
                paint.StrokeWidth = strokeWidth.Size;
            };

            lineCapJoinSample = view.FindViewById<ExtensibleView>(Resource.Id.LineCapJoinSample);
            lineCapJoinSample.Drawing += (canvas) =>
            {
                paint.LineCapJoinSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 150.0f));
            };
            lineCap = view.FindViewById<Spinner>(Resource.Id.LineCap);
            lineCap.ItemSelected += (o, e) =>
            {
                paint.LineCap = (LineCap)lineCap.SelectedItemPosition;
                lineCapJoinSample.Invalidate();
            };
            lineJoin = view.FindViewById<Spinner>(Resource.Id.LineJoin);
            lineJoin.ItemSelected += (o, e) =>
            {
                paint.LineJoin = (LineJoin)lineJoin.SelectedItemPosition;
                lineCapJoinSample.Invalidate();
            };

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
                paint.MiterLimit = miterLimitDeci.Value / 10.0f;
            };

            fillRuleSample = view.FindViewById<ExtensibleView>(Resource.Id.FillRuleSample);
            fillRule = view.FindViewById<Spinner>(Resource.Id.FillRule);
            fillRuleSample.Drawing += (canvas) =>
            {
                paint.FillRuleSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 150.0f));
            };
            fillRule.ItemSelected += (o, e) =>
            {
                paint.FillRule = (FillRule)fillRule.SelectedItemPosition;
                fillRuleSample.Invalidate();
            };

            dialog = Util.CreateDialog(activity, Resource.String.PaintOptions, view,
                () => ok?.Invoke(paint), null);
        }

        public void Show(Paint paint)
        {
            this.paint = new Paint(paint);
            strokeColor.Color = paint.StrokeColor;
            fillColor.Color = paint.FillColor;
            strokeWidth.Size = paint.StrokeWidth;
            lineCap.SetSelection((int)paint.LineCap);
            lineJoin.SetSelection((int)paint.LineJoin);
            miterLimitDeci.Value = (int)Math.Round(paint.MiterLimit * 10.0f);
            fillRule.SetSelection((int)paint.FillRule);

            dialog.Show();
        }
    }
}