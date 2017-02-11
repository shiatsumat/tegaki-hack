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
        DrawableView editcanvas;
        ImageButton undoButton, redoButton, saveButton;
        ImageButton freehandButton, lineButton, circleButton, ovalButton;
        ImageButton[] shapeButtons;
        Editor editor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Edit);
            editcanvas = FindViewById<DrawableView>(Resource.Id.Editcanvas);
            undoButton = FindViewById<ImageButton>(Resource.Id.Undo);
            redoButton = FindViewById<ImageButton>(Resource.Id.Redo);
            redoButton = FindViewById<ImageButton>(Resource.Id.Redo);
            saveButton = FindViewById<ImageButton>(Resource.Id.Save);
            freehandButton = FindViewById<ImageButton>(Resource.Id.Freehand);
            lineButton = FindViewById<ImageButton>(Resource.Id.Line);
            circleButton = FindViewById<ImageButton>(Resource.Id.Circle);
            ovalButton = FindViewById<ImageButton>(Resource.Id.Oval);
            shapeButtons = new ImageButton[] { freehandButton, lineButton, circleButton, ovalButton };
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

            freehandButton.Click += (o, e) => { SetActiveShapeButton(freehandButton); editor.ChangeShapeCreator(ShapeCreator.Freehand); };
            lineButton.Click += (o, e) => { SetActiveShapeButton(lineButton); editor.ChangeShapeCreator(ShapeCreator.Line); };
            circleButton.Click += (o, e) => { SetActiveShapeButton(circleButton); editor.ChangeShapeCreator(ShapeCreator.Circle); };
            ovalButton.Click += (o, e) => { SetActiveShapeButton(ovalButton); editor.ChangeShapeCreator(ShapeCreator.Oval); };
            SetActiveShapeButton(freehandButton);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        void SetActiveShapeButton(ImageButton activeShapeButton)
        {
            foreach (var shapeButton in shapeButtons)
            {
                shapeButton.BackgroundTintList = shapeButton != activeShapeButton ? null :
                    ColorStateList.ValueOf(NativeColor.Orange);
            }
        }
    }
}