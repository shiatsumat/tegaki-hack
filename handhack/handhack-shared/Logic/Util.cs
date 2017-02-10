using System;
using System.Collections.Generic;
using System.Text;

namespace handhack
{
    public static class UtilStatic
    {
        public static T LoopGet<T>(this T[] array, int index)
        {
            index = index < 0 ? index % array.Length : -((-index) % array.Length);
            return array[index];
        }
    }
}
