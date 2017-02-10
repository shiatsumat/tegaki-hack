namespace handhack
{
    public partial interface IShapeCreation
    {
        void Touchdown(Point<Internal> p);
        void Touchmove(Point<Internal> p);
        void Touchup(Point<Internal> p);
    }
}