using System;
using System.Collections.Generic;
using System.Linq;
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

        public static string[] SplitByNewLine(this string text)
        {
            return text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public static string JoinIntoString(this IEnumerable<string> items)
        {
            return items.JoinIntoString("");
        }

        public static string JoinIntoString(this IEnumerable<string> items, string separator)
        {
            return string.Join(separator, items.ToArray());
        }

        public static string NormaliseIndent(this string text)
        {
            var whitespaceRegexp = new Regex(@"^([\s]+)", RegexOptions.Compiled);
            var lines = text.SplitByNewLine();

            lines = RemoveBlankOrWhitespaceLinesAtStart(lines);
            lines = RemoveBlankOrWhitespaceLinesAtEnd(lines);

            var removableWhitespaceCharCount =
                lines
                    .Select(x => whitespaceRegexp.Match(x).Groups[0].Length)
                    .Min();

            return lines
                .Select(x => x.Substring(removableWhitespaceCharCount))
                .JoinIntoString("\r\n");
        }

        static string[] RemoveBlankOrWhitespaceLinesAtEnd(IEnumerable<string> lines)
        {
            return RemoveBlankOrWhitespaceLinesAtStart(lines.Reverse())
                .Reverse()
                .ToArray();
        }

        static string[] RemoveBlankOrWhitespaceLinesAtStart(IEnumerable<string> lines)
        {
            return lines
                .SkipWhile(x => x.Trim() == "")
                .ToArray();
        }
    }
}