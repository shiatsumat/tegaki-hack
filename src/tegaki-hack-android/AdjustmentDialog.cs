using System;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Android.Content.Res;
using Android.Views;
using NativeColor = Android.Graphics.Color;

namespace tegaki_hack
{

    public class AdjustmentDialog
    {
        View view;
        AlertDialog dialog;
        Adjustment adjustment;

        Activity activity;
        Action<Adjustment> ok;

        Spinner xAdjustment, yAdjustment;
        CheckBox adjustAngle;
        NumberPicker rightAngleDivision;
        CheckBox adjustLength;

        public static Task<AdjustmentDialog> AsyncNew(Activity activity, Action<Adjustment> ok)
        {
            return Task.Run<AdjustmentDialog>(() => new AdjustmentDialog(activity, ok));
        }
        public AdjustmentDialog(Activity activity, Action<Adjustment> ok)
        {
            this.activity = activity; this.ok = ok;
            view = activity.LayoutInflater.Inflate(Resource.Layout.AdjustmentDialog, null);
        }

        public void Show(Adjustment adjustment)
        {
            if (dialog == null) Initialize();
            SetAdjustment(adjustment);
            dialog.Show();
        }

        void SetAdjustment(Adjustment adjustment)
        {
            this.adjustment = new Adjustment(adjustment);
            xAdjustment.SetSelection((int)adjustment.XAdjustment);
            yAdjustment.SetSelection((int)adjustment.YAdjustment);
            adjustAngle.Checked = adjustment.DoesAdjustAngle;
            rightAngleDivision.Value = adjustment.RightAngleDivision;
            adjustLength.Checked = adjustment.DoesAdjustLength;
        }

        void Initialize()
        {
            InitializeCoordinateAdjustments();
            InitializeAngleAdjustment();
            InitializeLengthAdjustment();
            InitializeDialog(activity, ok);
        }
        void InitializeCoordinateAdjustments()
        {
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
        }
        void InitializeAngleAdjustment()
        {
            adjustAngle = view.FindViewById<CheckBox>(Resource.Id.AdjustAngle);
            adjustAngle.CheckedChange += (o, e) =>
            {
                adjustment.DoesAdjustAngle = adjustAngle.Checked;
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
        }
        void InitializeLengthAdjustment()
        {
            adjustLength = view.FindViewById<CheckBox>(Resource.Id.AdjustLength);
            adjustLength.CheckedChange += (o, e) =>
            {
                adjustment.DoesAdjustLength = adjustLength.Checked;
                SetAvailability();
            };
        }
        void InitializeDialog(Activity activity, Action<Adjustment> ok)
        {
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
    }
}