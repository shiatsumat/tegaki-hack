using System.Collections.Generic;
using static System.Math;

namespace handhack
{
    public static partial class UtilStatic
    {
        public const float EPS = 1e-4f;
        public static float ToAbs(this float oldvalue, float value)
        {
            if (oldvalue > 0) return Abs(value);
            else return -Abs(value);
        }
        public static int LoopIndex<T>(this List<T> list, int index)
        {
            return index < 0 ? index % list.Count : -((-index) % list.Count);
        }
        public static T LoopGet<T>(this List<T> list, int index)
        {
            return list[list.LoopIndex(index)];
        }
        public static void LoopSet<T>(this List<T> list, int index, T value)
        {
            list[list.LoopIndex(index)] = value;
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
        public static string svgName(string name)
        {
            return "{http://www.w3.org/2000/svg}" + name;
        }
    }
}
