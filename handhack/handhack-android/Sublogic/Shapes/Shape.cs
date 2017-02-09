using Android.Graphics;

namespace handhack_android
{
    public partial interface Shape
    {
        void Draw(Canvas canvas, Transform<Internal, External> transform);
    }
}