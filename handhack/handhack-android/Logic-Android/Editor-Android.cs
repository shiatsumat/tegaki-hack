using System.Collections.Generic;
using Android.Graphics;
using NativeColor = Android.Graphics.Color;

namespace handhack
{
    public partial class Editor : IDrawable
    {
        Bitmap secondBitmap;
        Canvas secondCanvas;

        void ResetSecondCanvas()
        {
            secondBitmap = null;
            secondCanvas = null;
        }
        void SetupSecondCanvas(int width, int height, Transform<Internal, External> transform)
        {
            if (secondCanvas == null || secondCanvas.Width != width || secondCanvas.Height != height)
            {
                secondBitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Rgb565);
                secondCanvas = new Canvas(secondBitmap);
                secondCanvas.DrawColor(NativeColor.White);

                SetGrid();
                secondCanvas.Draw(grid, transform);
            }

            if (undrawnShapes.Count > 0)
            {
                setUndoAbility(true);
                foreach (var shape in undrawnShapes)
                {
                    secondCanvas.Draw(shape, transform);
                }
                if (drawnShapes.Count == 0)
                {
                    var temp = drawnShapes;
                    drawnShapes = undrawnShapes;
                    undrawnShapes = temp;
                }
                else
                {
                    foreach (var shape in undrawnShapes) drawnShapes.Add(shape);
                    undrawnShapes.Clear();
                }
            }
        }
        public void Draw(Canvas canvas, Transform<Internal, External> transform)
        {
            SetupSecondCanvas(canvas.Width, canvas.Height, transform);
            canvas.DrawBitmap(secondBitmap, 0, 0, null);
            if (shapeCreator != null && shapeCreator.shape != null)
            {
                canvas.Draw(shapeCreator.shape, transform);
            }
        }
    }
}