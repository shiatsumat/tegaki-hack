using System.Collections.Generic;
using System.IO;
using Android.Graphics;
using Android.Widget;
using Android.Views;
using Android.App;
using NativeColor = Android.Graphics.Color;

namespace tegaki_hack
{
    public partial class Editor
    {
        CustomApplication application;
        EditActivity activity;

        LinearLayout header;
        ExtensibleView editcanvas;
        ImageButton undoButton, redoButton, clearButton, saveButton;
        Dictionary<EShapeCreatorFamily, ImageButton> shapeButtons;
        Dictionary<EShapeCreator, int> icons;
        ImageButton adjustmentButton, paintButton;

        Bitmap secondBitmap;
        Canvas secondCanvas;

        public Editor(EditActivity activity)
        {
            this.activity = activity;
            application = (CustomApplication)activity.Application;
            activity.SetContentView(Resource.Layout.Edit);

            InitializeFirst();

            InitializeHeader();
            InitializeEditCanvas();
            InitializeUndoRedoClear();
            InitializeSave();
            InitializeShapes();
            InitializeCircle();
            InitializeAdjustment();
            InitializePaint();

            InitializeLast();

            application.editButton.Text = "";
        }

        void InitializeHeader()
        {
            header = activity.FindViewById<LinearLayout>(Resource.Id.Header);
            header.Click += (o, e) =>
            {
                ToggleShapeCreatorFamily(EShapeCreatorFamily.None);
            };
        }
        void InitializeEditCanvas()
        {
            editcanvas = activity.FindViewById<ExtensibleView>(Resource.Id.Editcanvas);

            editcanvas.Touch += (o, e) =>
            {
                var p = new Point<External>(e.Event.GetX(), e.Event.GetY());
                switch (e.Event.Action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Down:
                        Touch(TouchEvent.Down, p);
                        break;
                    case MotionEventActions.Move:
                        Touch(TouchEvent.Move, p);
                        break;
                    case MotionEventActions.Up:
                        Touch(TouchEvent.Up, p);
                        break;
                }
            };
            editcanvas.LayoutChange += (o, e) =>
            {
                DealWithLayoutChange(new DPoint<External>(editcanvas.Width, editcanvas.Height));
            };
            editcanvas.Drawing += (canvas) =>
            {
                Draw(canvas);
            };
        }
        void InitializeUndoRedoClear()
        {
            undoButton = activity.FindViewById<ImageButton>(Resource.Id.Undo);
            redoButton = activity.FindViewById<ImageButton>(Resource.Id.Redo);
            clearButton = activity.FindViewById<ImageButton>(Resource.Id.Clear);

            undoButton.Click += (o, e) => Undo();
            redoButton.Click += (o, e) => Redo();
            clearButton.Click += (o, e) => Clear();
        }
        void InitializeSave()
        {
            saveButton = activity.FindViewById<ImageButton>(Resource.Id.Save);

            var view = activity.LayoutInflater.Inflate(Resource.Layout.SvgDialog, null);
            var dialogBuilder = new AlertDialog.Builder(activity);
            var text = view.FindViewById<TextView>(Resource.Id.SvgDialogText);
            dialogBuilder.SetTitle("Svg Output");
            dialogBuilder.SetView(view);
            dialogBuilder.SetPositiveButton("OK", (s, a) => { });
            var dialog = dialogBuilder.Create();
            saveButton.Click += (o, e) =>
            {
                var writer = new StringWriter();
                GetSvg().Save(writer);
                var svgString = writer.ToString();
                text.Text = svgString;
                dialog.Show();
                Util.DebugPrint(svgString);
            };
        }
        void InitializeShapes()
        {
            shapeButtons = new Dictionary<EShapeCreatorFamily, ImageButton>();
            shapeButtons[EShapeCreatorFamily.Freehand] = activity.FindViewById<ImageButton>(Resource.Id.Freehand);
            shapeButtons[EShapeCreatorFamily.Line] = activity.FindViewById<ImageButton>(Resource.Id.Line);
            shapeButtons[EShapeCreatorFamily.Circle] = activity.FindViewById<ImageButton>(Resource.Id.Circle);
            shapeButtons[EShapeCreatorFamily.Text] = activity.FindViewById<ImageButton>(Resource.Id.Text);

            icons = new Dictionary<EShapeCreator, int>();

            icons[EShapeCreator.Freehand] = Resource.Drawable.FreehandIcon;

            icons[EShapeCreator.Line] = Resource.Drawable.LineIcon;
            icons[EShapeCreator.Arc] = Resource.Drawable.ArcIcon;
            icons[EShapeCreator.Polyline] = Resource.Drawable.PolylineIcon;

            icons[EShapeCreator.Circle] = Resource.Drawable.CircleIcon;
            icons[EShapeCreator.Ellipse] = Resource.Drawable.EllipseIcon;
            icons[EShapeCreator.Square] = Resource.Drawable.SquareIcon;
            icons[EShapeCreator.Rectangle] = Resource.Drawable.RectangleIcon;
            icons[EShapeCreator.RegularPolygon] = Resource.Drawable.RegularPolygonIcon;
            icons[EShapeCreator.Polygon] = Resource.Drawable.PolygonIcon;

            icons[EShapeCreator.Text] = Resource.Drawable.TextIcon;
            icons[EShapeCreator.FancyText] = Resource.Drawable.FancyTextIcon;

            foreach (var key in shapeButtons.Keys)
            {
                shapeButtons[key].Click += (o, e) =>
                {
                    ToggleShapeCreatorFamily(key);
                };
            }
        }
        void InitializeCircle()
        {
            var view = activity.LayoutInflater.Inflate(Resource.Layout.CircleDialog, null);
            var nRegularPolygonPicker = view.FindViewById<NumberPicker>(Resource.Id.NRegularPolygon);
            int nRegularPolygon = 0;

            nRegularPolygonPicker.MinValue = 3;
            nRegularPolygonPicker.MaxValue = 50;
            nRegularPolygonPicker.WrapSelectorWheel = false;

            nRegularPolygonPicker.ValueChanged += (o, e) =>
            {
                nRegularPolygon = nRegularPolygonPicker.Value;
            };

            var dialog = Util.CreateDialog(activity, Resource.String.CircleFamilyOptions, view, () =>
                 {
                     if (nRegularPolygon != settings.NRegularPolygon)
                     {
                         settings.NRegularPolygon = nRegularPolygon;
                         if (eShapeCreator == EShapeCreator.RegularPolygon) ResetShapeCreator();
                     }
                 }, null);
            shapeButtons[EShapeCreatorFamily.Circle].LongClick += (o, e) =>
            {
                ResetShapeCreator();
                nRegularPolygon = settings.NRegularPolygon;
                nRegularPolygonPicker.Value = nRegularPolygon;

                dialog.Show();
            };
        }
        void InitializeAdjustment()
        {
            adjustmentButton = activity.FindViewById<ImageButton>(Resource.Id.Adjustment);

            adjustmentButton.Click += (o, e) =>
            {
                settings.DoesAdjust = !settings.DoesAdjust;
                adjustmentButton.Activate(settings.DoesAdjust);
            };

            var dialog = new AdjustmentDialog(activity, (adjustment) =>
            {
                settings.Adjustment = adjustment;
            });
            adjustmentButton.LongClick += (o, e) => dialog.Show(settings.Adjustment);
        }
        void InitializePaint()
        {
            paintButton = activity.FindViewById<ImageButton>(Resource.Id.Paint);
            var dialog = new PaintDialog(activity, (paint) =>
            {
                if (!settings.Paint.Equals(paint))
                {
                    settings.Paint = paint;
                    ResetShapeCreator();
                }
            });
            paintButton.Click += (o, e) => dialog.Show(settings.Paint);
        }

        public void Destroy()
        {
            application.editButton.Text = activity.GetString(Resource.String.Edit);
            application.savedShapes = drawnShapes;
        }

        partial void Redisplay()
        {
            editcanvas.Invalidate();
        }
        partial void SetUndoAbility(bool b)
        {
            undoButton.Enabled = b;
        }
        partial void SetRedoAbility(bool b)
        {
            redoButton.Enabled = b;
        }
        partial void SetClearAbility(bool b)
        {
            clearButton.Enabled = b;
        }
        partial void SetActiveness()
        {
            foreach (var key in shapeButtons.Keys)
            {
                shapeButtons[key].Activate(key == eShapeCreatorFamily);
            }
        }
        partial void SetIcons()
        {
            foreach (var key in shapeButtons.Keys)
            {
                shapeButtons[key].SetImageResource(icons[shapeDictionary[key][shapes[key]]]);
            }
        }

        partial void ResetSecondCanvas()
        {
            secondBitmap = Bitmap.CreateBitmap((int)realsize.Dx, (int)realsize.Dy, Bitmap.Config.Rgb565);
            secondCanvas = new Canvas(secondBitmap);
            secondCanvas.DrawColor(NativeColor.White);

            Grid().Draw(secondCanvas, transform);

            MoveDrawnShapesToUndrawn();
            DrawUndrawnShapesOnSecondCanvas();
        }
        void DrawUndrawnShapesOnSecondCanvas()
        {
            foreach (var shape in undrawnShapes) shape.Draw(secondCanvas, transform);
            MoveUndrawnShapesToDrawn();
        }
        void Draw(Canvas canvas)
        {
            DrawUndrawnShapesOnSecondCanvas();
            canvas.DrawBitmap(secondBitmap, 0, 0, null);
            shapeCreator?.Draw(canvas, transform);
        }
    }
}