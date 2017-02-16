using System;
using Android.App;
using Android.Widget;
using Android.Content.Res;
using NativeColor = Android.Graphics.Color;

namespace tegaki_hack
{
    public class AdjustmentDialog
    {
        AlertDialog dialog;

        Adjustment adjustment;

        Spinner xAdjustment, yAdjustment;
        CheckBox adjustAngle;
        NumberPicker rightAngleDivision;
        CheckBox adjustLength;

        public AdjustmentDialog(Activity activity, Action<Adjustment> ok)
        {
            var view = activity.LayoutInflater.Inflate(Resource.Layout.AdjustmentDialog, null);

            xAdjustment = view.FindViewById<Spinner>(Resource.Id.XAdjustment);
            xAdjustment.ItemSelected += (o, e) =>
            {
                adjustment.XAdjustment = (CoordinateAdjustment)xAdjustment.SelectedItemPosition;
                SetAvailability();
            };
            yAdjustment = view.FindViewById<Spinner>(Resource.Id.YAdjustment);
            yAdjustment.ItemSelected += (o, e) =>
            {
                adjustment.YAdjustment = (CoordinateAdjustment)yAdjustment.SelectedItemPosition;
                SetAvailability();
            };

            adjustAngle = view.FindViewById<CheckBox>(Resource.Id.AdjustAngle);
            adjustAngle.CheckedChange += (o, e) =>
            {
                adjustment.AdjustAngle = adjustAngle.Checked;
                SetAvailability();
            };

            rightAngleDivision = view.FindViewById<NumberPicker>(Resource.Id.RightAngleDivision);
            rightAngleDivision.MinValue = 1;
            rightAngleDivision.MaxValue = 90;
            rightAngleDivision.WrapSelectorWheel = false;
            rightAngleDivision.ValueChanged += (o, e) =>
            {
                adjustment.RightAngleDivision = rightAngleDivision.Value;
            };

            adjustLength = view.FindViewById<CheckBox>(Resource.Id.AdjustLength);
            adjustLength.CheckedChange += (o, e) =>
            {
                adjustment.AdjustLength = adjustLength.Checked;
                SetAvailability();
            };

            dialog = Util.CreateDialog(activity, Resource.String.AdjustmentOptions, view,
               () => ok(adjustment), null);
        }

        void SetAvailability()
        {
            adjustAngle.SetTextColor(adjustment.AngleAdjustmentAvailable ?
                ColorStateList.ValueOf(NativeColor.Black) : ColorStateList.ValueOf(NativeColor.LightGray));
            adjustLength.SetTextColor(adjustment.LengthAdjustmentAvailable ?
                ColorStateList.ValueOf(NativeColor.Black) : ColorStateList.ValueOf(NativeColor.LightGray));
        }

        public void Show(Adjustment adjustment)
        {
            this.adjustment = new Adjustment(adjustment);
            xAdjustment.SetSelection((int)adjustment.XAdjustment);
            yAdjustment.SetSelection((int)adjustment.YAdjustment);
            adjustAngle.Checked = adjustment.AdjustAngle;
            rightAngleDivision.Value = adjustment.RightAngleDivision;
            adjustLength.Checked = adjustment.AdjustLength;

            dialog.Show();
        }
    }
}