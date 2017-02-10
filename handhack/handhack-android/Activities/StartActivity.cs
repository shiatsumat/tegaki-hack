using Android.App;
using Android.Widget;
using Android.OS;

namespace handhack
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
	public class StartActivity : Activity
	{
		Button starteditButton;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

            SetContentView(Resource.Layout.Start);
            starteditButton = FindViewById<Button>(Resource.Id.Startedit);

            starteditButton.Click += (o, e) => { StartActivity(typeof(EditActivity)); };
		}
	}
}

