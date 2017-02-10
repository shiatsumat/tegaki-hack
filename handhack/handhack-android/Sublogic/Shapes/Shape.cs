using Android.Graphics;

namespace handhack
{
    public partial interface Shape
    {
        void Draw(Canvas canvas, Transform<Internal, External> transform);
    }
}