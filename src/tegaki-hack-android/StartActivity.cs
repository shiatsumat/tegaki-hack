using Android.App;
using Android.Widget;
using Android.OS;

namespace tegaki_hack
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/CustomTheme")]
	public class StartActivity : Activity
	{
		Button starteditButton;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

            SetContentView(Resource.Layout.Start);
            starteditButton = FindViewById<Button>(Resource.Id.StartEdit);

            starteditButton.Click += (o, e) => { StartActivity(typeof(EditActivity)); };
		}
    }
}

