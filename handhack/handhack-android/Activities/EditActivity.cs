using System.IO;
using Android.OS;
using Android.App;
using Android.Widget;
using Android.Views;
using Android.Content.Res;
using NativeColor = Android.Graphics.Color;

namespace handhack
{
    [Activity(Label = "EditActivity", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class EditActivity : Activity
    {
        ExtensibleView editcanvas;
        ImageButton undoButton, redoButton, saveButton;
        ImageButton[] shapeButtons;
        ImageButton strictButton;
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
            strictButton = FindViewById<ImageButton>(Resource.Id.Strict);
            shapeButtons = new ImageButton[] {
                FindViewById<ImageButton>(Resource.Id.Freehand),
                FindViewById<ImageButton>(Resource.Id.Line),
                FindViewById<ImageButton>(Resource.Id.Oval),
                FindViewById<ImageButton>(Resource.Id.Rectangle),
                FindViewById<ImageButton>(Resource.Id.RegularPolygon)
            };

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

            SetActiveShapeButton(shapeButtons[0]);
            for (var i = 0; i < shapeButtons.Length; i++)
            {
                var _i = i;
                var eShapeCreator = (EShapeCreator)_i;
                shapeButtons[_i].Click += (o, e) =>
                {
                    SetActiveShapeButton(shapeButtons[_i]);
                    editor.ChangeShapeCreator(eShapeCreator);
                };
            }

            strictButton.Click += (o, e) =>
            {
                editor.settings.strict = !editor.settings.strict;
                strictButton.Activate(editor.settings.strict);
            };

            {
                var view = LayoutInflater.Inflate(Resource.Layout.RegularPolygonDialog, null);
                var dialogBuilder = new AlertDialog.Builder(this);
                var numberPicker = view.FindViewById<NumberPicker>(Resource.Id.RegularPolygonDialogNumberPicker);
                dialogBuilder.SetTitle("Regular Polygon Option");
                dialogBuilder.SetView(view);
                numberPicker.MinValue = 3;
                numberPicker.MaxValue = 50;
                numberPicker.WrapSelectorWheel = false;
                dialogBuilder.SetPositiveButton("OK", (s, a) =>
                {
                    editor.nRegularPolygon = numberPicker.Value;
                    editor.ResetShapeCreator(EShapeCreator.RegularPolygon);
                });
                var dialog = dialogBuilder.Create();
                shapeButtons[4].LongClick += (o, e) =>
                {
                    var n = editor.nRegularPolygon;
                    numberPicker.Value = n;
                    dialog.Show();
                };
            }

            {
                var view = LayoutInflater.Inflate(Resource.Layout.SvgDialog, null);
                var dialogBuilder = new AlertDialog.Builder(this);
                var text = view.FindViewById<TextView>(Resource.Id.SvgDialogText);
                dialogBuilder.SetTitle("Svg Output");
                dialogBuilder.SetView(view);
                dialogBuilder.SetPositiveButton("OK", (s,a)=> { });
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
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        void SetActiveShapeButton(ImageButton activeShapeButton)
        {
            foreach (var shapeButton in shapeButtons)
            {
                shapeButton.Activate(shapeButton == activeShapeButton);
            }
        }
    }
}