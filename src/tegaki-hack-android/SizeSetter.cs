using System;
using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using Android.Util;

namespace tegaki_hack
{
    public class SizeSetter : LinearLayout
    {
        NumberPicker centi;
        Spinner pers;
        public event Action SizeChanged;

        public SizeEither Size
        {
            get { return new SizeEither(centi.Value / 100.0f, pers.SelectedItemPosition == 0); }
            set { centi.Value = (int)Math.Round(value.Value * 100.0f); pers.SetSelection(value.IsInternal ? 0 : 1); }
        }

        public SizeSetter(Context context, IAttributeSet attrs) :
                base(context, attrs)
        { Initialize(); }
        public SizeSetter(Context context, IAttributeSet attrs, int defStyleAttr) :
                base(context, attrs, defStyleAttr)
        { Initialize(); }
        void Initialize()
        {
            Inflate(Context, Resource.Layout.SizeSetter, this);

            centi = FindViewById<NumberPicker>(Resource.Id.Centi);
            pers = FindViewById<Spinner>(Resource.Id.Pers);
            centi.MinValue = 1;
            centi.MaxValue = 10000;
            var centstrings = new List<string>();
            for (int i = centi.MinValue; i <= centi.MaxValue; i++)
            {
                centstrings.Add(string.Format("{0:f2}", i / 100.0));
            }
            centi.SetDisplayedValues(centstrings.ToArray());
            centi.WrapSelectorWheel = false;
            centi.ValueChanged += (o, e) => SizeChanged?.Invoke();
            pers.ItemSelected += (o, e) => SizeChanged?.Invoke();
        }
    }
}