using Android.Graphics;

namespace handhack
{
    public partial class Editor : IDrawable
    {
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            canvas.Draw(grid, transform);
            foreach (var shape in shapes)
            {
                canvas.Draw(shape, transform);
            }
            if (shapeCreator != null && shapeCreator.shape != null)
            {
                canvas.Draw(shapeCreator.shape, transform);
            }
        }
    }
}