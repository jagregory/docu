using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DrDoc.Associations;

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
            return new DocNamespace(MemberName.FromNamespace(ns));
        }

        protected DocType Type<T>()
        {
            return new DocType(MemberName.FromType(typeof(T)));
        }
    }
}