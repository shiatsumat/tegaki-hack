namespace handhack
{
	class Square
	{
		public Point<Internal> center;
		public double sidelength;

		public Square(Point<Internal> center, double sidelength)
		{
			this.center = center; this.sidelength = sidelength;
		}
	}
	class Rectangle
	{
		public Point<Internal> upperleft, lowerright;

		public Rectangle(Point<Internal> upperleft, Point<Internal> lowerright)
		{
			this.upperleft = upperleft; this.lowerright = lowerright;
		}
	}
}