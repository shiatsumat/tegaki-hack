using Android.App;
using Android.Widget;
using Android.OS;

namespace handhack_android
{
	[Activity(MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		Button editButton;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			editButton = FindViewById<Button>(Resource.Id.edit);

			editButton.Click += (o, e) => { StartActivity(typeof(EditActivity)); };
		}
	}
}

