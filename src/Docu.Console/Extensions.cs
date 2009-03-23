using System.Collections.Generic;

namespace Docu
{
    public static class Extensions
    {
        public static IList<T> ToTail<T>(this IList<T> list)
        {
            var shortened = new List<T>(list);

            if (shortened.Count > 0)
                shortened.RemoveAt(0);

            return shortened;
        }
    }
}