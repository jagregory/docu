using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Tests
{
    internal static class TestExtensions
    {
        public static IList<Namespace> to_namespaces(this string[] namespaces)
        {
            var list = new List<Namespace>();

            foreach (var ns in namespaces)
                list.Add(to_namespace(ns));

            return list;
        }

        public static InlineText to_text(this string text)
        {
            return new InlineText(text);
        }

        public static Namespace to_namespace(this string ns)
        {
            return new Namespace(Identifier.FromNamespace(ns));
        }

        public static DeclaredType to_type(this Type type)
        {
            return type.to_type_in_namespace(type.Namespace.to_namespace());
        }

        public static DeclaredType to_type<T>(this T instance)
        {
            return typeof(T).to_type();
        }

        public static DeclaredType to_type_in_namespace(this Type type, Namespace ns)
        {
            var declaredType = new DeclaredType(Identifier.FromType(type), ns);

            ns.AddType(declaredType);

            return declaredType;
        }

        public static MethodParameter to_method_parameter_called(this DeclaredType type, string name)
        {
            return new MethodParameter(name, type);
        }

        public static Method method_for<T>(this T instance, Expression<Func<T, object>> method_signature)
        {
            return new Method(Identifier.FromMethod(Method(method_signature), typeof(T)), null);
        }

        public static Method method_for<T>(this T instance, Expression<Action<T>> method_signature)
        {
            return new Method(Identifier.FromMethod(Method(method_signature), typeof(T)), null);
        }

        public static Property property_for<T>(this T instance, Expression<Func<T, object>> property_signature)
        {
            return new Property(Identifier.FromProperty(Property(property_signature), typeof(T)), null);
        }

        public static T setup<T>(this T instance, Action<T> setup)
        {
            setup(instance);

            return instance;
        }

        private static PropertyInfo Property<T>(Expression<Func<T, object>> propertyAction)
        {
            return ((MemberExpression)propertyAction.Body).Member as PropertyInfo;
        }

        private static MethodInfo Method<T>(Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            if (method.IsGenericMethod)
                return method.GetGenericMethodDefinition();

            return method;
        }

        private static MethodInfo Method<T>(Expression<Func<T, object>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            if (method.IsGenericMethod)
                return method.GetGenericMethodDefinition();

            return method;
        }
    }
}