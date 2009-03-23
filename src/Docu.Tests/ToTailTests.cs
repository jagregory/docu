using System.Collections.Generic;
using NUnit.Framework;

namespace Docu.Tests
{
    [TestFixture]
    public class ToTailTests
    {
        [Test]
        public void ShouldReturnTailOfList()
        {
            var list = new List<string> { "one", "two", "three" };
            var tail = list.ToTail();

            tail.Count.ShouldEqual(2);
            tail[0].ShouldEqual("two");
            tail[1].ShouldEqual("three");
        }

        [Test]
        public void ShouldReturnEmptyListIfNoMoreTail()
        {
            var list = new List<string> { "one" };
            var tail = list.ToTail();

            tail.Count.ShouldEqual(0);
        }

        [Test]
        public void ShouldReturnEmptyListIfUsedOnEmptyList()
        {
            var list = new List<string>();
            var tail = list.ToTail();

            tail.Count.ShouldEqual(0);
        }
    }
}