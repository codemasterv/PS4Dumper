using System.Collections.Generic;
using PS4wpfDumper;


namespace PS4wpfDumper.Util
{
    public static class DictionaryExtensions
    {
        public static V GetOrDefault<K, V>(this Dictionary<K, V> d, K key, V def = default(V))
        {
            if (d.ContainsKey(key)) return d[key];
            return def;
        }
    }

    public static class ArrayExtensions
    {
        public static T[] Fill<T>(this T[] arr, T val)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = val;
            }
            return arr;
        }
    }

    public static class ByteArrayExtensions
    {
        public static string ToHexCompact(this byte[] b)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var x in b) sb.AppendFormat("{0:X2}", x);
            return sb.ToString();
        }
    }
#if !CORE
    public static class TupleExtension
    {
        public static void Deconstruct<T1, T2>(this System.Tuple<T1, T2> tuple, out T1 item1, out T2 item2)
        {
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }
    }

#endif
}
