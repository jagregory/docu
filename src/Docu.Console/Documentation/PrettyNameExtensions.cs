using System;
using System.Reflection;
using System.Text;

namespace Docu.Documentation
{
    public static class PrettyNameExtensions
    {
        public static string GetPrettyName(this Type type)
        {
            string specialName = GetSpecialName(type);

            if (specialName != null)
            {
                return specialName;
            }

            if (type.IsNested)
            {
                return type.Name;
            }

            if (type.IsGenericType)
            {
                var sb = new StringBuilder();

                sb.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
                sb.Append("<");

                foreach (Type argument in type.GetGenericArguments())
                {
                    sb.Append(argument.GetPrettyName());
                    sb.Append(", ");
                }

                sb.Length -= 2;
                sb.Append(">");

                return sb.ToString();
            }

            return type.Name;
        }

        public static string GetPrettyName(this MethodBase method)
        {
            if (method.IsConstructor)
            {
                return method.DeclaringType.GetPrettyName();
            }

            if (method.IsGenericMethod)
            {
                var sb = new StringBuilder();
                string name = method.Name;

                if (name.Contains("`"))
                {
                    name = method.Name.Substring(0, method.Name.IndexOf('`'));
                }

                sb.Append(name);
                sb.Append("<");

                foreach (Type argument in method.GetGenericArguments())
                {
                    sb.Append(argument.Name);
                    sb.Append(", ");
                }

                sb.Length -= 2;
                sb.Append(">");

                return sb.ToString();
            }

            return method.Name;
        }

        public static string GetSpecialName(Type type)
        {
            if (type == typeof (string))
            {
                return "string";
            }

            if (type == typeof (int))
            {
                return "int";
            }

            if (type == typeof (uint))
            {
                return "uint";
            }

            if (type == typeof (long))
            {
                return "long";
            }

            if (type == typeof (ulong))
            {
                return "ulong";
            }

            if (type == typeof (double))
            {
                return "double";
            }

            if (type == typeof (float))
            {
                return "float";
            }

            if (type == typeof (decimal))
            {
                return "decimal";
            }

            if (type == typeof (short))
            {
                return "short";
            }

            if (type == typeof (ushort))
            {
                return "ushort";
            }

            if (type == typeof (void))
            {
                return "void";
            }

            if (type == typeof (bool))
            {
                return "bool";
            }

            if (type == typeof (object))
            {
                return "object";
            }

            return null;
        }
    }
}
