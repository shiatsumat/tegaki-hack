namespace handhack_android
{
	class Circle
	{
		public Point<Internal> center;
		public double radius;

		public Circle(Point<Internal> center, double radius)
		{
			this.center = center; this.radius = radius;
		}
	}
	class Oval
	{
		public Point<Internal> center;
		public DPoint<Internal> radius;

		public Oval(Point<Internal> center, DPoint<Internal> radius)
		{
			this.center = center; this.radius = radius;
		}
	}
}