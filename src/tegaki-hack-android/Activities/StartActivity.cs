using Android.App;
using Android.Widget;
using Android.OS;

namespace tegaki_hack
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/CustomTheme")]
    public class StartActivity : Activity
    {
        CustomApplication application;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            application = (CustomApplication)Application;

            SetContentView(Resource.Layout.Start);
            application.editButton = FindViewById<Button>(Resource.Id.StartEdit);

            application.editButton.Click += (o, e) =>
            {
                application.editButton.Text = GetString(Resource.String.PleaseWait);
                StartActivity(typeof(EditActivity));
            };
        }
    }
}

