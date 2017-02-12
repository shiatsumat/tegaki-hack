using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using static System.Math;

namespace handhack
{
    [Activity(Label = "EditActivity", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class EditActivity : Activity
    {
        ExtensibleView editcanvas;
        ImageButton undoButton, redoButton, saveButton;
        ImageButton[] shapeButtons;
        ImageButton strictModeButton, paintButton;
        Editor editor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Edit);
            editcanvas = FindViewById<ExtensibleView>(Resource.Id.Editcanvas);
            undoButton = FindViewById<ImageButton>(Resource.Id.Undo);
            redoButton = FindViewById<ImageButton>(Resource.Id.Redo);
            redoButton = FindViewById<ImageButton>(Resource.Id.Redo);
            saveButton = FindViewById<ImageButton>(Resource.Id.Save);
            shapeButtons = new ImageButton[] {
                FindViewById<ImageButton>(Resource.Id.Freehand),
                FindViewById<ImageButton>(Resource.Id.Line),
                FindViewById<ImageButton>(Resource.Id.Oval),
                FindViewById<ImageButton>(Resource.Id.Rectangle),
                FindViewById<ImageButton>(Resource.Id.RegularPolygon)
            };
            strictModeButton = FindViewById<ImageButton>(Resource.Id.StrictMode);
            paintButton = FindViewById<ImageButton>(Resource.Id.Paint);

            editor = new Editor(new DPoint<Internal>(30, 30),
                () => { editcanvas.Invalidate(); },
                (b) => { undoButton.Enabled = b; },
                (b) => { redoButton.Enabled = b; });

            editcanvas.Touch += (o, e) =>
            {
                var p = new Point<External>(e.Event.GetX(), e.Event.GetY());
                switch (e.Event.Action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Down:
                        editor.Touch(Touchevent.Down, p);
                        break;
                    case MotionEventActions.Move:
                        editor.Touch(Touchevent.Move, p);
                        break;
                    case MotionEventActions.Up:
                        editor.Touch(Touchevent.Up, p);
                        break;
                }
            };
            editcanvas.LayoutChange += (o, e) =>
            {
                editor.DealWithLayoutChange(new DPoint<External>(editcanvas.Width, editcanvas.Height));
            };
            editcanvas.Drawing += (canvas) =>
            {
                editor.Draw(canvas);
            };

            undoButton.Click += (o, e) => editor.Undo();
            redoButton.Click += (o, e) => editor.Redo();
            {
                var view = LayoutInflater.Inflate(Resource.Layout.SvgDialog, null);
                var dialogBuilder = new AlertDialog.Builder(this);
                var text = view.FindViewById<TextView>(Resource.Id.SvgDialogText);
                dialogBuilder.SetTitle("Svg Output");
                dialogBuilder.SetView(view);
                dialogBuilder.SetPositiveButton("OK", (s, a) => { });
                var dialog = dialogBuilder.Create();
                saveButton.Click += (o, e) =>
                {
                    var writer = new StringWriter();
                    editor.GetSvg().Save(writer);
                    var svgString = writer.ToString();
                    text.Text = svgString;
                    dialog.Show();
                    System.Diagnostics.Debug.WriteLine(svgString);
                };
            }

            shapeButtons[0].Activate(true);
            for (var i = 0; i < shapeButtons.Length; i++)
            {
                var _i = i;
                var eShapeCreator = (EShapeCreator)_i;
                shapeButtons[_i].Click += (o, e) =>
                {
                    foreach (var shapeButton in shapeButtons)
                    {
                        shapeButton.Activate(shapeButton == shapeButtons[_i]);
                    }
                    editor.SetShapeCreator(eShapeCreator);
                };
            }
            {
                var view = LayoutInflater.Inflate(Resource.Layout.RegularPolygonDialog, null);
                var dialogBuilder = new AlertDialog.Builder(this);
                var numberPicker = view.FindViewById<NumberPicker>(Resource.Id.NRegularPolygon);
                dialogBuilder.SetTitle(Resource.String.RegularPolygonOptions);
                dialogBuilder.SetView(view);
                numberPicker.MinValue = 3;
                numberPicker.MaxValue = 50;
                numberPicker.WrapSelectorWheel = false;
                dialogBuilder.SetPositiveButton("OK", (s, a) =>
                {
                    if (numberPicker.Value != editor.settings.nRegularPolygon)
                    {
                        editor.settings.nRegularPolygon = numberPicker.Value;
                    }
                });
                var dialog = dialogBuilder.Create();
                shapeButtons[4].LongClick += (o, e) =>
                {
                    editor.ResetShapeCreator(EShapeCreator.RegularPolygon);
                    numberPicker.Value = editor.settings.nRegularPolygon;
                    dialog.Show();
                };
            }

            strictModeButton.Click += (o, e) =>
            {
                editor.settings.strictMode = !editor.settings.strictMode;
                strictModeButton.Activate(editor.settings.strictMode);
            };
            {
                var view = LayoutInflater.Inflate(Resource.Layout.StrictModeDialog, null);
                var dialogBuilder = new AlertDialog.Builder(this);
                var rightAngleDivision = view.FindViewById<NumberPicker>(Resource.Id.RightAngleDivision);
                dialogBuilder.SetTitle(Resource.String.StrictModeOptions);
                dialogBuilder.SetView(view);
                rightAngleDivision.MinValue = 1;
                rightAngleDivision.MaxValue = 90;
                rightAngleDivision.WrapSelectorWheel = false;
                dialogBuilder.SetPositiveButton("OK", (s, a) =>
                {
                    editor.settings.rightAngleDivision = rightAngleDivision.Value;
                });
                var dialog = dialogBuilder.Create();
                strictModeButton.LongClick += (o, e) =>
                {
                    rightAngleDivision.Value = editor.settings.rightAngleDivision;
                    dialog.Show();
                };
            }
            {
                var view = LayoutInflater.Inflate(Resource.Layout.PaintDialog, null);
                var dialogBuilder = new AlertDialog.Builder(this);
                var strokeColor = view.FindViewById<ColorPicker>(Resource.Id.StrokeColor);
                var strokeWidthPers = view.FindViewById<Spinner>(Resource.Id.StrokeWidthPers);
                var strokeWidthCent = view.FindViewById<NumberPicker>(Resource.Id.StrokeWidthCent);
                var fillColor = view.FindViewById<ColorPicker>(Resource.Id.FillColor);
                var linecap = view.FindViewById<Spinner>(Resource.Id.Linecap);
                var linejoin = view.FindViewById<Spinner>(Resource.Id.Linejoin);
                dialogBuilder.SetTitle(Resource.String.PaintOptions);
                dialogBuilder.SetView(view);
                strokeWidthCent.MinValue = 1;
                strokeWidthCent.MaxValue = 10000;
                strokeWidthCent.WrapSelectorWheel = false;
                dialogBuilder.SetPositiveButton("OK", (s, a) =>
                {
                    editor.settings.paint.strokeColor = strokeColor.color;
                    editor.settings.paint.strokeWidth = new SizeEither(strokeWidthCent.Value / 100.0f, strokeWidthPers.SelectedItemPosition == 0);
                    editor.settings.paint.fillColor = fillColor.color;
                    editor.settings.paint.linecap = (Linecap)linecap.SelectedItemPosition;
                    editor.settings.paint.linejoin = (Linejoin)linejoin.SelectedItemPosition;
                });
                var dialog = dialogBuilder.Create();
                paintButton.Click += (o, e) =>
                {
                    editor.ResetShapeCreator();
                    strokeColor.color = editor.settings.paint.strokeColor;
                    strokeWidthPers.SetSelection(editor.settings.paint.strokeWidth.isInternal ? 0 : 1);
                    strokeWidthCent.Value = (int)Round(editor.settings.paint.strokeWidth.value * 100.0f);
                    fillColor.color = editor.settings.paint.fillColor;
                    linecap.SetSelection((int)editor.settings.paint.linecap);
                    linejoin.SetSelection((int)editor.settings.paint.linejoin);
                    dialog.Show();
                };
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}