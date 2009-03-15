using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DrDoc.Model;
using DrDoc.Parsing;

namespace DrDoc.Tests
{
    public abstract class BaseFixture
    {
        protected MethodInfo Method<T>(Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            if (method.IsGenericMethod)
                return method.GetGenericMethodDefinition();

            return method;
        }

        protected PropertyInfo Property<T>(Expression<Func<T, object>> propertyAction)
        {
            return ((MemberExpression)propertyAction.Body).Member as PropertyInfo;
        }

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
            return new DocNamespace(Identifier.FromNamespace(ns));
        }

        protected DocType Type<T>()
        {
            return new DocType(Identifier.FromType(typeof(T)));
        }

        protected IList<IDocumentationMember> DocMembers(params Type[] types)
        {
            var documentableMembers = new DocumentableMemberFinder();

            return documentableMembers.GetMembersForDocumenting(types);
        }
    }
}