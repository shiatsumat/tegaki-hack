using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views;

namespace handhack
{
    [Activity(Label = "EditActivity", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class EditActivity : Activity
    {
        Editcanvas editcanvas;
        ImageButton undoButton, redoButton;
        Editor editor;
        Transform<Internal, External> transform;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Edit);
            editcanvas = FindViewById<Editcanvas>(Resource.Id.Editcanvas);
            undoButton = FindViewById<ImageButton>(Resource.Id.Undo);
            redoButton = FindViewById<ImageButton>(Resource.Id.Redo);
            editor = new Editor(new DPoint<Internal>(30, 30));

            editcanvas.Touch += (o, e) =>
            {
                transform = new Transform<Internal, External>(editcanvas.Height / 30f);
                var p = new Point<External>(e.Event.XPrecision, e.Event.YPrecision).Untransform(transform);
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
            editcanvas.onDraw += (canvas) =>
            {
                transform = new Transform<Internal, External>(editcanvas.Height / 30);
                canvas.Draw(editor, transform);
            };
            undoButton.Click += (o, e) => editor.Undo();
            redoButton.Click += (o, e) => editor.Redo();
            editor.update += () => { editcanvas.Invalidate(); };
            editor.setUndoAbility += (b) => { undoButton.Enabled = b; };
            editor.setRedoAbility += (b) => { redoButton.Enabled = b; };

            editor.Update();
        }
    }
}