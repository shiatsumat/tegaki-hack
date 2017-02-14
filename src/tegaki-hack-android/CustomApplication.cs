using System;
using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Android.Widget;

namespace tegaki_hack
{
    [Application]
    public class CustomApplication : Application
    {
        public Button editButton;
        public List<IShape> savedShapes;

        public CustomApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }
    }
}