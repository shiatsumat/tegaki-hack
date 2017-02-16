using System;
using System.Collections.Generic;
using System.IO;
using Android.Graphics;
using Android.Widget;
using Android.Views;
using Android.App;
using Android.Content.Res;
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

            var dialog = Util.CreateDialog(activity, Resource.String.CirlceFamilyOptions, view, () =>
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

            var view = activity.LayoutInflater.Inflate(Resource.Layout.AdjustmentDialog, null);
            var adjustment = new Adjustment();

            Action setAvailability = null;

            var xAdjustment = view.FindViewById<Spinner>(Resource.Id.XAdjustment);
            xAdjustment.ItemSelected += (o, e) =>
            {
                adjustment.XAdjustment = (CoordinateAdjustment)xAdjustment.SelectedItemPosition;
                setAvailability();
            };
            var yAdjustment = view.FindViewById<Spinner>(Resource.Id.YAdjustment);
            yAdjustment.ItemSelected += (o, e) =>
            {
                adjustment.YAdjustment = (CoordinateAdjustment)yAdjustment.SelectedItemPosition;
                setAvailability();
            };

            var adjustAngle = view.FindViewById<CheckBox>(Resource.Id.AdjustAngle);
            adjustAngle.CheckedChange += (o, e) =>
            {
                adjustment.AdjustAngle = adjustAngle.Checked;
                setAvailability();
            };

            var rightAngleDivision = view.FindViewById<NumberPicker>(Resource.Id.RightAngleDivision);
            rightAngleDivision.MinValue = 1;
            rightAngleDivision.MaxValue = 90;
            rightAngleDivision.WrapSelectorWheel = false;
            rightAngleDivision.ValueChanged += (o, e) =>
            {
                adjustment.RightAngleDivision = rightAngleDivision.Value;
            };

            var adjustLength = view.FindViewById<CheckBox>(Resource.Id.AdjustLength);
            adjustLength.CheckedChange += (o, e) =>
            {
                adjustment.AdjustLength = adjustLength.Checked;
                setAvailability();
            };

            var dialog = Util.CreateDialog(activity, Resource.String.AdjustmentOptions, view, () =>
            {
                settings.Adjustment = adjustment;
            }, null);

            setAvailability = () =>
            {
                adjustAngle.SetTextColor(adjustment.AngleAdjustmentAvailable ?
                    ColorStateList.ValueOf(NativeColor.Black) : ColorStateList.ValueOf(NativeColor.LightGray));
                adjustLength.SetTextColor(adjustment.LengthAdjustmentAvailable ?
                    ColorStateList.ValueOf(NativeColor.Black) : ColorStateList.ValueOf(NativeColor.LightGray));
            };

            adjustmentButton.LongClick += (o, e) =>
            {
                adjustment = new Adjustment(settings.Adjustment);
                xAdjustment.SetSelection((int)adjustment.XAdjustment);
                yAdjustment.SetSelection((int)adjustment.YAdjustment);
                adjustAngle.Checked = adjustment.AdjustAngle;
                rightAngleDivision.Value = adjustment.RightAngleDivision;
                adjustLength.Checked = adjustment.AdjustLength;

                dialog.Show();
            };
        }
        void InitializePaint()
        {
            paintButton = activity.FindViewById<ImageButton>(Resource.Id.Paint);

            var view = activity.LayoutInflater.Inflate(Resource.Layout.PaintDialog, null);
            var paint = new Paint();

            var strokeColor = view.FindViewById<ColorSetter>(Resource.Id.StrokeColor);
            strokeColor.ColorChanged += () =>
            {
                paint.StrokeColor = strokeColor.Color;
            };

            var strokeWidth = view.FindViewById<SizeSetter>(Resource.Id.StrokeWidth);
            strokeWidth.SizeChanged += () =>
             {
                 paint.StrokeWidth = strokeWidth.Size;
             };

            var fillColor = view.FindViewById<ColorSetter>(Resource.Id.FillColor);
            fillColor.ColorChanged += () =>
            {
                paint.FillColor = fillColor.Color;
            };

            var lineCaplineJoinView = view.FindViewById<ExtensibleView>(Resource.Id.LineCapLineJoinView);
            lineCaplineJoinView.Drawing += (canvas) =>
            {
                paint.LineCapLineJoinSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 150.0f));
            };

            var lineCap = view.FindViewById<Spinner>(Resource.Id.LineCap);
            lineCap.ItemSelected += (o, e) =>
            {
                paint.LineCap = (LineCap)lineCap.SelectedItemPosition;
                lineCaplineJoinView.Invalidate();
            };

            var lineJoin = view.FindViewById<Spinner>(Resource.Id.LineJoin);
            lineJoin.ItemSelected += (o, e) =>
            {
                paint.LineJoin = (LineJoin)lineJoin.SelectedItemPosition;
                lineCaplineJoinView.Invalidate();
            };

            var miterLimitDeci = view.FindViewById<NumberPicker>(Resource.Id.MiterLimitDeci);
            miterLimitDeci.MinValue = 0;
            miterLimitDeci.MaxValue = 1000;
            var deciStrings = new List<string>();
            for (int i = miterLimitDeci.MinValue; i <= miterLimitDeci.MaxValue; i++)
            {
                deciStrings.Add(string.Format("{0:f1}", i / 10.0));
            }
            miterLimitDeci.SetDisplayedValues(deciStrings.ToArray());
            miterLimitDeci.WrapSelectorWheel = false;
            miterLimitDeci.ValueChanged += (o, e) =>
            {
                paint.MiterLimit = miterLimitDeci.Value / 10.0f;
            };

            var fillRuleView = view.FindViewById<ExtensibleView>(Resource.Id.FillRuleView);
            var fillRule = view.FindViewById<Spinner>(Resource.Id.FillRule);
            fillRuleView.Drawing += (canvas) =>
            {
                paint.FillRuleSample().Draw(canvas,
                    new Transform<Internal, External>(canvas.Width / 150.0f));
            };
            fillRule.ItemSelected += (o, e) =>
            {
                paint.FillRule = (FillRule)fillRule.SelectedItemPosition;
                fillRuleView.Invalidate();
            };

            var dialog = Util.CreateDialog(activity, Resource.String.PaintOptions, view, () =>
            {
                if (!paint.Equals(settings.Paint))
                {
                    settings.Paint = paint;
                    ResetShapeCreator();
                }
            }, null);

            paintButton.Click += (o, e) =>
            {
                paint = new Paint(settings.Paint);
                strokeColor.Color = paint.StrokeColor;
                strokeWidth.Size = paint.StrokeWidth;
                fillColor.Color = paint.FillColor;
                lineCap.SetSelection((int)paint.LineCap);
                lineJoin.SetSelection((int)paint.LineJoin);
                miterLimitDeci.Value = (int)Math.Round(paint.MiterLimit * 10.0f);
                fillRule.SetSelection((int)paint.FillRule);

                dialog.Show();
            };
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