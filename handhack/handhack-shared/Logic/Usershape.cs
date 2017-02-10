namespace handhack
{
    public static partial class UsershapeStatic
    {
        public enum Shapesort { Linelike, Squarelike, Circlelike, Textlike }
        public const int shapesortNumber = 4;
        public static readonly int[] shapeNumbers = new int[] { 3, 4, 3, 2 };

        public enum Linelikeshape { Freehand, Line, Goodline }
        public enum Squarelikeshape { Square, Roundsquare, Rectangle, Roundrectangle }
        public enum Circlelikeshape { Circle, Oval, Arc }
        public enum Textlikeshape { Text, Fancytext }

        public static int NextShape(Shapesort shapesort, int nowshape)
        {
            return (nowshape + 1) % shapeNumbers[(int)shapesort];
        }
    }
}
