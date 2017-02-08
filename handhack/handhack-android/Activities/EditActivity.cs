
using Android.App;
using Android.OS;
using Android.Widget;

namespace handhack_android
{
	[Activity(Label = "EditActivity")]
	public class EditActivity : Activity
	{
		Button undoButton, redoButton;
		Button curveButton, lineButton, goodlineButton;
		Button squareButton, rectangleButton;
		Button circleButton, ovalButton, arcButton;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Edit);

			undoButton = FindViewById<Button>(Resource.Id.undo);
			redoButton = FindViewById<Button>(Resource.Id.redo);

			curveButton = FindViewById<Button>(Resource.Id.curve);
			lineButton = FindViewById<Button>(Resource.Id.line);
			goodlineButton = FindViewById<Button>(Resource.Id.goodline);
			squareButton = FindViewById<Button>(Resource.Id.square);
			rectangleButton = FindViewById<Button>(Resource.Id.rectangle);
			circleButton = FindViewById<Button>(Resource.Id.circle);
			ovalButton = FindViewById<Button>(Resource.Id.oval);
			arcButton = FindViewById<Button>(Resource.Id.arc);
		}
	}
}