using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content.Res;
using Android.Views;

using static handhack.UsershapeStatic;

using NativeColor = Android.Graphics.Color;

namespace handhack
{
    [Activity(Label = "EditActivity", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class EditActivity : Activity
    {
        public ImageButton undoButton, redoButton;
        public ImageButton[] shapeButtons;

        public Editcanvas editcanvas;

        public Shapesort shapesort = Shapesort.Linelike;
        public int[] shapes = new int[] {
            (int)Linelikeshape.Freehand, (int)Squarelikeshape.Square, (int)Circlelikeshape.Circle, (int)Textlikeshape.Text };
        public Linelikeshape linelikeshape { get { return (Linelikeshape)shapes[0]; } set { shapes[0] = (int)value; } }
        public Squarelikeshape squarelikeshape { get { return (Squarelikeshape)shapes[1]; } set { shapes[1] = (int)value; } }
        public Circlelikeshape circlelikeshape { get { return (Circlelikeshape)shapes[2]; } set { shapes[2] = (int)value; } }
        public Textlikeshape textlikeshape { get { return (Textlikeshape)shapes[3]; } set { shapes[3] = (int)value; } }

        public readonly int[][] shapeicons = new int[][] {
            new int[] { Resource.Drawable.Freehand, Resource.Drawable.Line, Resource.Drawable.Goodline },
            new int[] { Resource.Drawable.Square, Resource.Drawable.Roundsquare, Resource.Drawable.Rectangle, Resource.Drawable.Roundrectangle },
            new int[] { Resource.Drawable.Circle, Resource.Drawable.Oval, Resource.Drawable.Arc },
            new int[] { Resource.Drawable.Text, Resource.Drawable.Fancytext }
        };

        public Transform<Internal, External> transform;

        public ShapeCreation shapeCreation;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Edit);

            undoButton = FindViewById<ImageButton>(Resource.Id.Undo);
            redoButton = FindViewById<ImageButton>(Resource.Id.Redo);

            shapeButtons = new ImageButton[] {
                FindViewById<ImageButton>(Resource.Id.Linelike),
                FindViewById<ImageButton>(Resource.Id.Squarelike),
                FindViewById<ImageButton>(Resource.Id.Circlelike),
                FindViewById<ImageButton>(Resource.Id.Textlike)
            };
            for (int i = 0; i < shapesortNumber; i++)
            {
                var _shapesort = (Shapesort)i;
                var _i = i;
                shapeButtons[i].Click += (o, e) =>
                {
                    if (shapesort != _shapesort)
                    {
                        shapesort = _shapesort; UpdateShapesort();
                    }
                    else
                    {
                        shapes[_i] = NextShape(shapesort, shapes[_i]); UpdateShape(shapesort);
                    }
                };
            }

            UpdateShapesort();

            editcanvas = FindViewById<Editcanvas>(Resource.Id.Editcanvas);
            transform = new Transform<Internal, External>(editcanvas.Height / 30);
            editcanvas.Touch += (o, e) =>
            {
                var p = new Point<External>(e.Event.XPrecision, e.Event.YPrecision).Untransform(transform);
                switch (e.Event.Action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Down:
                        shapeCreation.Touchdown(p);
                        break;
                    case MotionEventActions.Move:
                        shapeCreation.Touchmove(p);
                        break;
                    case MotionEventActions.Up:
                        shapeCreation.Touchup(p);
                        break;
                }
            };
            editcanvas.onDraw += (canvas) =>
            {
                var paint = new Android.Graphics.Paint();
                paint.Color = NativeColor.Black;
                paint.TextSize = 100;
                canvas.DrawText(string.Format("{0}x{1}", canvas.Width, canvas.Height), 100, 100, paint);
            };
        }

        void UpdateShapesort()
        {
            if (shapesort < 0 || shapesortNumber <= (int)shapesort)
            {
                throw new IndexOutOfRangeException("Shapesort is invalid");
            }
            for (int i = 0; i < shapesortNumber; i++)
            {
                shapeButtons[i].BackgroundTintList = (i != (int)shapesort) ? null :
                    ColorStateList.ValueOf(NativeColor.Orange);
            }
        }

        void UpdateShape(Shapesort shapesort)
        {
            var i = (int)shapesort;
            if (shapes[i] < 0 || shapes[i] >= shapeNumbers[i])
            {
                throw new IndexOutOfRangeException(string.Format("{0} is invalid", shapesort.ToString()));
            }
            else
            {
                shapeButtons[i].SetImageResource(shapeicons[i][shapes[i]]);
            }
        }
    }
}