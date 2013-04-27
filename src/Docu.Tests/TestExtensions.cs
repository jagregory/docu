using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Tests
{
    internal static class TestExtensions
    {
        public static IList<Namespace> to_namespaces(this IEnumerable<string> namespaces)
        {
            return namespaces.Select(to_namespace).ToList();
        }

        public static InlineText to_text(this string text)
        {
            return new InlineText(text);
        }

        public static Namespace to_namespace(this string ns)
        {
            return new Namespace(IdentifierFor.Namespace(ns));
        }

        public static DeclaredType to_type(this Type type)
        {
            return type.to_type_in_namespace(type.Namespace.to_namespace());
        }

        static DeclaredType to_type_in_namespace(this Type type, Namespace ns)
        {
            var declaredType = new DeclaredType(IdentifierFor.Type(type), ns);

            ns.AddType(declaredType);

            return declaredType;
        }

        public static MethodParameter to_method_parameter_called(this DeclaredType type, string name)
        {
            return new MethodParameter(name, type);
        }

        public static Method method_for<T>(this T instance, Expression<Action<T>> method_signature)
        {
            return new Method(IdentifierFor.Method(Method(method_signature), typeof (T)), null);
        }

        public static Property property_for<T>(this T instance, Expression<Func<T, object>> property_signature)
        {
            return new Property(IdentifierFor.Property(Property(property_signature), typeof (T)), null);
        }

        public static T Setup<T>(this T instance, Action<T> setup)
        {
            setup(instance);
            return instance;
        }

        static PropertyInfo Property<T>(Expression<Func<T, object>> propertyAction)
        {
            return ((MemberExpression) propertyAction.Body).Member as PropertyInfo;
        }

        static MethodInfo Method<T>(Expression<Action<T>> methodAction)
        {
            MethodInfo method = ((MethodCallExpression) methodAction.Body).Method;

            if (method.IsGenericMethod)
                return method.GetGenericMethodDefinition();

            return method;
        }
    }
}
