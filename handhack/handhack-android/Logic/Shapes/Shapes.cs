namespace handhack_android
{
    public static class Shapes
    {
        public enum Linelike { Freehand, Line, Goodline }
        public static int linelikeNumber = 3;
        public enum Squarelike { Square, Rectangle }
        public static int squarelikeNumber = 2;
        public enum Circlelike { Circle, Oval, Arc }
        public static int circlelikeNumber = 3;
        public static Linelike NextLinelike(Linelike linelike)
        {
            return (Linelike)(((int)linelike + 1) % linelikeNumber);
        }
        public static Squarelike NextSquarelike(Squarelike squarelike)
        {
            return (Squarelike)(((int)squarelike + 1) % squarelikeNumber);
        }
        public static Circlelike NextCirclelike(Circlelike circlelike)
        {
            return (Circlelike)(((int)circlelike + 1) % circlelikeNumber);
        }
    }
}