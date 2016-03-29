using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PocketProxy.Utils
{
    public static class Extensions
    {
        public static byte ToAngle(this float fl)
        {
            return (byte) (fl/360f*256);
        }

        public static int ToFixedPoint(this float fl)
        {
            return (int)Math.Round(fl * 32);
        }

        public static int ToFixedPoint(this double fl)
        {
            return (int)Math.Round(fl * 32);
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' ||
                    c == '§' || c == '[' || c == ']' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static int SetBit(this int target, int field, bool value)
        {
            if (value)
            {
                return target | field;
            }
            return target & (~field);
        }

        public static bool GetBit(this int target, int field)
        {
            return (target & field) > 0;
        }

        public static Dictionary<TKey, TValue> CloneDictionairy<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> original)
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value);
            }
            return ret;
        }

        public static TValue[] CloneAsArray<TValue>(this List<TValue> original)
        {
            TValue[] t = new TValue[original.Count];
            original.CopyTo(0, t, 0, t.Length);

            return t;
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

		public static long NextLong(this Random random)
		{
			byte[] longBytes = new byte[8];
			random.NextBytes(longBytes);
			return BitConverter.ToInt64(longBytes, 0);
		}
	}
}