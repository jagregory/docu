using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Docu.Documentation;
using Docu.Parsing;
using Docu.Parsing.Model;
using Docu.Documentation;
using Docu.Parsing.Model;
using Docu.Parsing;
using DeclaredType=Docu.Documentation.DeclaredType;

namespace Docu.Tests
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

        protected IList<Docu.Documentation.Namespace> Namespaces(params string[] namespaces)
        {
            var list = new List<Docu.Documentation.Namespace>();

            foreach (var ns in namespaces)
            {
                list.Add(Namespace(ns));
            }

            return list;
        }

        protected Docu.Documentation.Namespace Namespace(string ns)
        {
            return new Docu.Documentation.Namespace(Identifier.FromNamespace(ns));
        }

        protected DeclaredType Type<T>(Namespace ns)
        {
            var type = new DeclaredType(Identifier.FromType(typeof(T)), ns);

            ns.AddType(type);

            return type;
        }

        protected IList<IDocumentationMember> DocMembers(params System.Type[] types)
        {
            var documentableMembers = new DocumentableMemberFinder();

            return documentableMembers.GetMembersForDocumenting(types);
        }
    }
}