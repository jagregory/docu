using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Docu
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static IList<T> ToTail<T>(this IList<T> list)
        {
            var shortened = new List<T>(list);

            if (shortened.Count > 0)
                shortened.RemoveAt(0);

            return shortened;
        }

        public static string TrimComment(this string text, bool first, bool last)
        {
            var regexp = new Regex(@"[\s]{0,}\r?\n[\s]{0,}");

            string prepared = regexp.Replace(text, "\r\n");

            if(first)
                prepared = prepared.TrimStart(' ', '\t', '\r', '\n');
            else
                prepared = prepared.TrimStart('\t', '\r', '\n');

            if(last)
                prepared = prepared.TrimEnd(' ', '\t', '\r', '\n');
            else
                prepared = prepared.TrimEnd('\t', '\r', '\n');

            return prepared;
        }
    }
}