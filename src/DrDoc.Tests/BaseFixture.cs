using System.Collections.Generic;
using DrDoc.Associations;

namespace DrDoc.Tests
{
    public abstract class BaseFixture
    {
        protected IList<DocNamespace> Namespaces(params string[] namespaces)
        {
            var list = new List<DocNamespace>();

            foreach (var ns in namespaces)
            {
                list.Add(Namespace(ns));
            }

            return list;
        }

        protected DocNamespace Namespace(string ns)
        {
            return new DocNamespace(MemberName.FromNamespace(ns));
        }

        protected DocType Type<T>()
        {
            return new DocType(MemberName.FromType(typeof(T)));
        }
    }
}