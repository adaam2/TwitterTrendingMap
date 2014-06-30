using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ikvm.lang;
using java.util;


namespace FinalUniProject.IKVM.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Converts Java list to generic .NET list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this java.util.List list) where T : class
        {
            var newList = new List<T>(list.size());
            for (var i = 0; i < list.size(); i++)
            {
                newList.Add((T)list.get(i));
            }
            return newList;
        }

        public static void ClearAddRange<T>(this IList<T> current, IEnumerable<T> collection) where T : class
        {
            current.Clear();
            foreach (var item in collection)
            {
                current.Add(item);
            }
        }


        /// <summary>
        /// Converts generic .NET list to Java list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static java.util.List ToJavaList<T>(this IList<T> list) where T : class
        {
            var newList = new java.util.ArrayList(list.Count);
            foreach (var t in list)
            {
                newList.Add(t);
            }
            return newList;
        }

        /// <summary>
        /// Converts <see cref="Map"/> to <see cref="Dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="map"></param>
        /// <returns></returns>
        //public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this Map map)
        //{
        //    //var asd = new string[] {"SearchArtists", "Artist"};
        //    //var qwe = new string[] {"SearchArtists", "Artist"};
        //    //var zxc = asd.Concat(qwe).ToArray();
        //    var dictionary = new Dictionary<TKey, TValue>(map.size());
        //    var entries = map.entrySet().toArray();
        //    for (var i = 0; i < dictionary.Count; i++)
        //    {
        //        var entry = (Map.Entry)entries[i];
        //        var key = entry.CastThis<TKey>();
        //        var value = entry.CastThis<TValue>();
        //        dictionary.Add(key, value);
        //    }
        //    return dictionary;
        //}

        public static T CastThis<T>(this Object obj)
        {
            return (T)obj;
        }

        public static double ToDouble(this java.lang.Double current)
        {
          
            return double.Parse(current.ToString(),NumberStyles.Any, new System.Globalization.CultureInfo("en-US") );
        }
    }
}