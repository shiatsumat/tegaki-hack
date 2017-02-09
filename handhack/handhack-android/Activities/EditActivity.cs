using System;
using Android.App;
using Android.OS;
using Android.Widget;
using static handhack_android.Shapes;

namespace handhack_android
{
    [Activity(Label = "EditActivity", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class EditActivity : Activity
    {
        public ImageButton undoButton, redoButton, linelikeButton, squarelikeButton, circlelikeButton;

        public Linelike linelike = Linelike.Freehand;
        public Squarelike squarelike = Squarelike.Square;
        public Circlelike circlelike = Circlelike.Circle;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Edit);

            undoButton = FindViewById<ImageButton>(Resource.Id.Undo);
            redoButton = FindViewById<ImageButton>(Resource.Id.Redo);
            linelikeButton = FindViewById<ImageButton>(Resource.Id.Linelike);
            linelikeButton.Click += (o, e) => {
                linelike = NextLinelike(linelike); UpdateLinelike();
            };
            squarelikeButton = FindViewById<ImageButton>(Resource.Id.Squarelike);
            squarelikeButton.Click += (o, e) => {
                squarelike = NextSquarelike(squarelike); UpdateSquarelike();
            };
            circlelikeButton = FindViewById<ImageButton>(Resource.Id.Circlelike);
            circlelikeButton.Click += (o, e) => {
                circlelike = NextCirclelike(circlelike); UpdateCirclelike();
            };
        }

        void UpdateLinelike()
        {
            switch (linelike)
            {
                case Linelike.Freehand:
                    linelikeButton.SetImageResource(Resource.Drawable.Freehand);
                    break;
                case Linelike.Line:
                    linelikeButton.SetImageResource(Resource.Drawable.Line);
                    break;
                case Linelike.Goodline:
                    linelikeButton.SetImageResource(Resource.Drawable.Goodline);
                    break;
                default:
                    throw new IndexOutOfRangeException("linelike is invalid");
            }
        }
        void UpdateSquarelike()
        {
            switch (squarelike)
            {
                case Squarelike.Square:
                    squarelikeButton.SetImageResource(Resource.Drawable.Square);
                    break;
                case Squarelike.Rectangle:
                    squarelikeButton.SetImageResource(Resource.Drawable.Rectangle);
                    break;
                default:
                    throw new IndexOutOfRangeException("squarelike is invalid");
            }
        }
        void UpdateCirclelike()
        {
            switch (circlelike)
            {
                case Circlelike.Circle:
                    circlelikeButton.SetImageResource(Resource.Drawable.Circle);
                    break;
                case Circlelike.Oval:
                    circlelikeButton.SetImageResource(Resource.Drawable.Oval);
                    break;
                case Circlelike.Arc:
                    circlelikeButton.SetImageResource(Resource.Drawable.Arc);
                    break;
                default:
                    throw new IndexOutOfRangeException("circlelike is invalid");
            }
        }
    }
}