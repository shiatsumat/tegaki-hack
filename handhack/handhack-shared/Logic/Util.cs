using System.Collections.Generic;

namespace handhack
{
    public static class UtilStatic
    {
        public static T LoopGet<T>(this List<T> list, int index)
        {
            index = index < 0 ? index % list.Count : -((-index) % list.Count);
            return list[index];
        }
        public static T Pop<T>(this List<T> list)
        {
            var res = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return res;
        }
        public static List<T> newList<T>(params T[] ts)
        {
            return new List<T>(ts);
        }
    }
    public delegate void BoolAction(bool b);
}
