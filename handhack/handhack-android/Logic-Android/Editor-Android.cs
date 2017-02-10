using Android.Graphics;

namespace handhack
{
    public partial class Editor : IDrawable
    {
        public void Draw<X>(Canvas canvas, Transform<Internal, X> transform) where X : External
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