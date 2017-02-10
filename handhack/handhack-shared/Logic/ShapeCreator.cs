using System;
using System.Collections.Generic;

namespace handhack
{
    public abstract partial class AShapeCreator
    {
        public Paint paint;
        public Action editted;
        public IShape shape;

        public abstract void Touch(Touchevent touchevent, Point<Internal> p);
    }
    public partial class FreehandCreator : AShapeCreator
    {
        public List<Point<Internal>> points;

        public FreehandCreator(Paint paint)
        {
            this.paint = paint;
        }

        override public void Touch(Touchevent touchevent, Point<Internal> p)
        {
        }
    }
}