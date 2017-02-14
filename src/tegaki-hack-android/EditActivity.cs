using Android.App;
using Android.OS;

namespace tegaki_hack
{
    [Activity(Label = "EditActivity", Theme = "@style/CustomTheme")]
    public class EditActivity : Activity
    {
        Editor editor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            editor = new Editor(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            editor.Destroy();
        }
    }
}