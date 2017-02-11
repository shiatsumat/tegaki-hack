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
                FindViewById<ImageButton>(Resource.Id.Rectangle)
            };
            for (var i = 0; i < shapeButtons.Length; i++)
            {
                var _i = i;
                shapeButtons[_i].Click += (o, e) =>
                {
                    SetActiveShapeButton(shapeButtons[_i]);
                    editor.ChangeShapeCreator((EShapeCreator)_i);
                };
            }
            strictButton.Click += (o, e) =>
            {
                editor.strict= !editor.strict;
                strictButton.Activate(editor.strict);
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
            saveButton.Click += (o, e) =>
            {
                var writer = new StringWriter();
                var svgdoc = editor.GetSvg();
                svgdoc.Save(writer);
                System.Diagnostics.Debug.Write(writer.ToString());
            };
            SetActiveShapeButton(shapeButtons[0]);
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