namespace handhack_android
{
    public partial interface ShapeCreation
    {
        void Touchdown(Point<Internal> p);
        void Touchmove(Point<Internal> p);
        void Touchup(Point<Internal> p);
    }
}