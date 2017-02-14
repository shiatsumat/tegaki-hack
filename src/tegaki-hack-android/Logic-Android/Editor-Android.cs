using System;
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

        ExtensibleView editcanvas;
        ImageButton undoButton, redoButton, saveButton;
        ImageButton[] shapeButtons;
        ImageButton adjustmentButton, paintButton;

        Bitmap secondBitmap;
        Canvas secondCanvas;

        public Editor(EditActivity activity)
        {
            this.activity = activity;

            application = (CustomApplication)activity.Application;

            activity.SetContentView(Resource.Layout.Edit);

            editcanvas = activity.FindViewById<ExtensibleView>(Resource.Id.Editcanvas);
            undoButton = activity.FindViewById<ImageButton>(Resource.Id.Undo);
            redoButton = activity.FindViewById<ImageButton>(Resource.Id.Redo);
            saveButton = activity.FindViewById<ImageButton>(Resource.Id.Save);

            shapeButtons = new ImageButton[] {
                activity.FindViewById<ImageButton>(Resource.Id.Freehand),
                activity.FindViewById<ImageButton>(Resource.Id.Line),
                activity.FindViewById<ImageButton>(Resource.Id.Oval),
                activity.FindViewById<ImageButton>(Resource.Id.Rectangle),
                activity.FindViewById<ImageButton>(Resource.Id.RegularPolygon)
            };
            adjustmentButton = activity.FindViewById<ImageButton>(Resource.Id.Adjustment);
            paintButton = activity.FindViewById<ImageButton>(Resource.Id.Paint);

            size = new DPoint<Internal>(30, 30);

            editcanvas.Touch += (o, e) =>
            {
                var p = new Point<External>(e.Event.GetX(), e.Event.GetY());
                switch (e.Event.Action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Down:
                        Touch(Touchevent.Down, p);
                        break;
                    case MotionEventActions.Move:
                        Touch(Touchevent.Move, p);
                        break;
                    case MotionEventActions.Up:
                        Touch(Touchevent.Up, p);
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

            undoButton.Click += (o, e) => Undo();
            redoButton.Click += (o, e) => Redo();

            InitializeSave();

            shapeButtons[0].Activate(true);
            for (var i = 0; i < shapeButtons.Length; i++)
            {
                var _i = i;
                var eShapeCreator = (EShapeCreator)_i;
                shapeButtons[_i].Click += (o, e) =>
                {
                    foreach (var shapeButton in shapeButtons)
                    {
                        shapeButton.Activate(shapeButton == shapeButtons[_i]);
                    }
                    SetShapeCreator(eShapeCreator);
                };
            }

            InitializeRegularPolygon();

            InitializeAdjustment();

            InitializePaint();

            application.editButton.Text = "";

            Initialize();
        }

        void InitializeSave()
        {
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

        void InitializeRegularPolygon()
        {
            var view = activity.LayoutInflater.Inflate(Resource.Layout.RegularPolygonDialog, null);
            var dialogBuilder = new AlertDialog.Builder(activity);
            var numberPicker = view.FindViewById<Android.Widget.NumberPicker>(Resource.Id.NRegularPolygon);
            dialogBuilder.SetTitle(Resource.String.RegularPolygonOptions);
            dialogBuilder.SetView(view);
            numberPicker.MinValue = 3;
            numberPicker.MaxValue = 50;
            numberPicker.WrapSelectorWheel = false;
            dialogBuilder.SetPositiveButton("OK", (s, a) =>
            {
                if (numberPicker.Value != settings.nRegularPolygon)
                {
                    settings.nRegularPolygon = numberPicker.Value;
                }
            });
            var dialog = dialogBuilder.Create();
            shapeButtons[4].LongClick += (o, e) =>
            {
                ResetShapeCreator(EShapeCreator.RegularPolygon);
                numberPicker.Value = settings.nRegularPolygon;
                dialog.Show();
            };
        }

        void InitializeAdjustment()
        {
            adjustmentButton.Click += (o, e) =>
            {
                settings.adjustment = !settings.adjustment;
                adjustmentButton.Activate(settings.adjustment);
            };

            var view = activity.LayoutInflater.Inflate(Resource.Layout.AdjustmentDialog, null);
            var dialogBuilder = new AlertDialog.Builder(activity);
            var rightAngleDivision = view.FindViewById<Android.Widget.NumberPicker>(Resource.Id.RightAngleDivision);
            dialogBuilder.SetTitle(Resource.String.AdjustmentOptions);
            dialogBuilder.SetView(view);
            rightAngleDivision.MinValue = 1;
            rightAngleDivision.MaxValue = 90;
            rightAngleDivision.WrapSelectorWheel = false;
            dialogBuilder.SetPositiveButton("OK", (s, a) =>
            {
                settings.rightAngleDivision = rightAngleDivision.Value;
            });
            var dialog = dialogBuilder.Create();
            adjustmentButton.LongClick += (o, e) =>
            {
                rightAngleDivision.Value = settings.rightAngleDivision;
                dialog.Show();
            };
        }

        void InitializePaint()
        {
            var view = activity.LayoutInflater.Inflate(Resource.Layout.PaintDialog, null);

            var dialogBuilder = new AlertDialog.Builder(activity);
            var strokeColor = view.FindViewById<ColorSetter>(Resource.Id.StrokeColor);
            var strokeWidthPers = view.FindViewById<Spinner>(Resource.Id.StrokeWidthPers);
            var strokeWidthCent = view.FindViewById<NumberPicker>(Resource.Id.StrokeWidthCent);
            var fillColor = view.FindViewById<ColorSetter>(Resource.Id.FillColor);
            var linecaplinejoinView = view.FindViewById<ExtensibleView>(Resource.Id.LinecapLinejoinView);
            var linecap = view.FindViewById<Spinner>(Resource.Id.Linecap);
            var linejoin = view.FindViewById<Spinner>(Resource.Id.Linejoin);
            var fillRuleView = view.FindViewById<ExtensibleView>(Resource.Id.FillRuleView);
            var fillRule = view.FindViewById<Spinner>(Resource.Id.FillRule);
            dialogBuilder.SetTitle(Resource.String.PaintOptions);
            dialogBuilder.SetView(view);

            strokeWidthCent.MinValue = 1;
            strokeWidthCent.MaxValue = 10000;
            var centstrings = new List<string>();
            for (int i = strokeWidthCent.MinValue; i <= strokeWidthCent.MaxValue; i++)
            {
                centstrings.Add(string.Format("{0:f2}", i / 100.0));
            }
            strokeWidthCent.SetDisplayedValues(centstrings.ToArray());
            strokeWidthCent.WrapSelectorWheel = false;

            strokeColor.ColorChanged += () =>
            {
                settings.paint.strokeColor = strokeColor.color;
            };

            Action strokeWidthChanged = () =>
            {
                settings.paint.strokeWidth = new SizeEither(strokeWidthCent.Value / 100.0f, strokeWidthPers.SelectedItemPosition == 0);
            };
            strokeWidthCent.ValueChanged += (o, e) => strokeWidthChanged();
            strokeWidthPers.ItemSelected += (o, e) => strokeWidthChanged();

            fillColor.ColorChanged += () =>
            {
                settings.paint.fillColor = fillColor.color;
            };

            linecaplinejoinView.Drawing += (canvas) =>
            {
                /* internally W 150 x H 50 */
                var transform = new Transform<Internal, External>(canvas.Width / 150.0f);
                var paint = new Paint(Color.Rgba(0x808080FF), new SizeEither(10.0f, true), default(Color),
                    settings.paint.linecap, settings.paint.linejoin);
                var polyline = new Polyline(paint, Util.NewList<Point<Internal>>(
                    new Point<Internal>(10, 10),
                    new Point<Internal>(60, 10),
                    new Point<Internal>(75, 40),
                    new Point<Internal>(90, 10),
                    new Point<Internal>(140, 10)));
                polyline.Draw(canvas, transform);
                paint.strokeColor = Color.Rgba(0xFFFFFFFF);
                paint.strokeWidth = new SizeEither(1.0f, true);
                paint.linecap = Linecap.Butt;
                paint.linejoin = Linejoin.Miter;
                polyline.Draw(canvas, transform);
            };
            linecap.ItemSelected += (o, e) =>
            {
                settings.paint.linecap = (Linecap)linecap.SelectedItemPosition;
                linecaplinejoinView.Invalidate();
            };
            linejoin.ItemSelected += (o, e) =>
            {
                settings.paint.linejoin = (Linejoin)linejoin.SelectedItemPosition;
                linecaplinejoinView.Invalidate();
            };

            fillRuleView.Drawing += (canvas) =>
            {
                /* internally W 150 x H 120 */
                var transform = new Transform<Internal, External>(canvas.Width / 150.0f);
                var paint = new Paint(Color.Rgba(0x404040FF), new SizeEither(3.0f, true), Color.Rgba(0x808080FF),
                    fillRule: settings.paint.fillRule);
                var polyline = new Polyline(paint, Util.NewList<Point<Internal>>(
                    new Point<Internal>(10, 110),
                    new Point<Internal>(50, 10),
                    new Point<Internal>(100, 60),
                    new Point<Internal>(50, 60),
                    new Point<Internal>(100, 10),
                    new Point<Internal>(140, 110)),
                    true);
                polyline.Draw(canvas, transform);
            };
            fillRule.ItemSelected += (o, e) =>
            {
                settings.paint.fillRule = (FillRule)fillRule.SelectedItemPosition;
                fillRuleView.Invalidate();
            };

            dialogBuilder.SetPositiveButton("OK", (s, a) => { });

            var dialog = dialogBuilder.Create();

            paintButton.Click += (o, e) =>
            {
                settings.paint = new Paint(settings.paint);
                ResetShapeCreator();
                strokeColor.color = settings.paint.strokeColor;
                strokeWidthPers.SetSelection(settings.paint.strokeWidth.isInternal ? 0 : 1);
                strokeWidthCent.Value = (int)Math.Round(settings.paint.strokeWidth.value * 100.0f);
                fillColor.color = settings.paint.fillColor;
                linecap.SetSelection((int)settings.paint.linecap);
                linejoin.SetSelection((int)settings.paint.linejoin);
                fillRule.SetSelection((int)settings.paint.fillRule);
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

        partial void ResetSecondCanvas()
        {
            secondBitmap = Bitmap.CreateBitmap((int)realsize.dx, (int)realsize.dy, Bitmap.Config.Rgb565);
            secondCanvas = new Canvas(secondBitmap);
            secondCanvas.DrawColor(NativeColor.White);

            SetGrid();
            grid.Draw(secondCanvas, transform);

            MoveDrawnShapesToUndrawn();
            DrawUndrawnShapesOnSecondCanvas();
        }
        void DrawUndrawnShapesOnSecondCanvas()
        {
            foreach (var shape in undrawnShapes) shape.Draw(secondCanvas, transform);
            MoveUndrawnShapesToDrawn();
        }
        public void Draw(Canvas canvas)
        {
            DrawUndrawnShapesOnSecondCanvas();
            canvas.DrawBitmap(secondBitmap, 0, 0, null);
            shapeCreator?.Draw(canvas, transform);
        }
    }
}