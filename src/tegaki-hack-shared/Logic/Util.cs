using System;
using System.Collections.Generic;

namespace tegaki_hack
{
    public enum TouchEvent { Down, Move, Up }

    public static partial class Util
    {
        public static void DebugPrint(string format, params object[] args)
        {
            System.Diagnostics.Debug.Print(format, args);
        }

        public const float EPS = 1e-4f;
        public static float AdjustAbs(this float oldvalue, float value)
        {
            if (oldvalue > 0) return Math.Abs(value);
            else return -Math.Abs(value);
        }

        public static T Nulling<T>(ref T x) where T : class
        {
            var temp = x;
            x = null;
            return temp;
        }

        public static int LoopIndex<T>(this IList<T> list, int index)
        {
            return index >= 0 ? index % list.Count : list.Count - (-index) % list.Count;
        }
        public static T LoopGet<T>(this IList<T> list, int index)
        {
            return list[list.LoopIndex(index)];
        }
        public static void LoopSet<T>(this IList<T> list, int index, T value)
        {
            list[list.LoopIndex(index)] = value;
        }
        public static T Pop<T>(this IList<T> list)
        {
            var res = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return res;
        }
        public static List<T> NewList<T>(params T[] ts)
        {
            return new List<T>(ts);
        }

        public static string SvgName(string name)
        {
            return "{http://www.w3.org/2000/svg}" + name;
        }

        public static bool ZeroOrOne(bool a, bool b, bool c)
        {
            return (!a && !b) || (!b && !c) || (!c && !a);
        }
    }
}
