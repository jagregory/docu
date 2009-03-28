using System.Collections.Generic;

namespace Docu.Tests.Parsing
{
    public class AssertionList<T>
    {
        private readonly IList<T> inner;

        public AssertionList(IEnumerable<T> list)
        {
            inner = new List<T>(list);
        }

        public T FirstItem
        {
            get { return inner[0]; }
        }
    }
}