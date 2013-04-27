using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Docu.Documentation;
using Docu.Parsing.Model;
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

        protected FieldInfo Field<T>(Expression<Func<T, object>> fieldAction)
        {
            return ((MemberExpression)fieldAction.Body).Member as FieldInfo;
        }

        protected EventInfo Event<T>(string name)
        {
            return typeof(T).GetEvent(name);
        }

        protected IList<Namespace> Namespaces(params string[] namespaces)
        {
            var list = new List<Namespace>();

            foreach (var ns in namespaces)
            {
                list.Add(Namespace(ns));
            }

            return list;
        }

        protected Namespace Namespace(string ns)
        {
            return new Namespace(IdentifierFor.Namespace(ns));
        }

        protected DeclaredType Type<T>(Namespace ns)
        {
            var type = new DeclaredType(IdentifierFor.Type(typeof(T)), ns);

            ns.AddType(type);

            return type;
        }
    }
}