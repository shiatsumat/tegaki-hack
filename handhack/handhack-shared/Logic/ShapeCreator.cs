using System;
using System.Collections.Generic;
using static handhack.UtilStatic;

namespace handhack
{
    public abstract partial class AShapeCreator
    {
        public Paint paint;
        public IShape shape;
        public Action edited, finish;
        public bool working;

        public AShapeCreator(Paint paint)
        {
            this.paint = new Paint(paint);
            Init();
        }
        public abstract void Touch(Touchevent touchevent, Point<Internal> p);
        public virtual void Init()
        {
            working = false;
            shape = null;
        }
        public virtual void Start()
        {
            working = true;
        }
        public virtual void Finish()
        {
            finish();
            Init();
        }
        public virtual void Bye()
        {
            finish();
        }
    }
    public partial class FreehandCreator : AShapeCreator
    {
        public List<Point<Internal>> points;

        public FreehandCreator(Paint paint) : base(paint) { }
        public override void Start()
        {
            base.Start();
            points = new List<Point<Internal>>();
            shape = new Polyline(paint, points, true);
        }
        public override void Touch(Touchevent touchevent, Point<Internal> p)
        {
            switch (touchevent)
            {
                case Touchevent.Down:
                    Start();
                    AddPoint(p);
                    break;
                case Touchevent.Move:
                    if (working) AddPoint(p);
                    break;
                case Touchevent.Up:
                    AddPoint(p);
                    Finish();
                    break;
            }
        }
        void AddPoint(Point<Internal> p)
        {
            if (points.Count > 0 && points[points.Count - 1].distance(p) < 1e-2) return;
            points.Add(p);
            edited();
        }
    }
    public partial class LineCreator : AShapeCreator
    {
        public LineCreator(Paint paint) : base(paint) { }
        public override void Start()
        {
            base.Start();
            shape = new Polyline(paint, newList(default(Point<Internal>), default(Point<Internal>)));
        }
        public override void Touch(Touchevent touchevent, Point<Internal> p)
        {
            switch (touchevent)
            {
                case Touchevent.Down:
                    Start();
                    ChangeFrom(p); ChangeTo(p);
                    break;
                case Touchevent.Move:
                    if (working) ChangeTo(p);
                    break;
                case Touchevent.Up:
                    ChangeTo(p);
                    Finish();
                    break;
            }
        }
        void ChangeFrom(Point<Internal> p)
        {
            ((Polyline)shape).points[0] = p;
            edited();
        }
        void ChangeTo(Point<Internal> p)
        {
            ((Polyline)shape).points[1] = p;
            edited();
        }
    }
    public partial class CircleCreator : AShapeCreator
    {
        public CircleCreator(Paint paint) : base(paint) { }
        public override void Start()
        {
            base.Start();
            shape = new Oval(paint, default(Point<Internal>), default(DPoint<Internal>));
        }
        public override void Touch(Touchevent touchevent, Point<Internal> p)
        {
            switch (touchevent)
            {
                case Touchevent.Down:
                    Start();
                    ChangeFrom(p); ChangeTo(p);
                    break;
                case Touchevent.Move:
                    if (working) ChangeTo(p);
                    break;
                case Touchevent.Up:
                    ChangeTo(p);
                    Finish();
                    break;
            }
        }
        void ChangeFrom(Point<Internal> p)
        {
            ((Oval)shape).center = p;
            edited();
        }
        void ChangeTo(Point<Internal> p)
        {
            var r = ((Oval)shape).center.distance(p);
            ((Oval)shape).radii = new DPoint<Internal>(r, r);
            edited();
        }
    }
}