using System;
using System.Reflection;
using System.Text;

namespace DrDoc.Documentation
{
    public static class PrettyNameExtensions
    {
        public static string GetSpecialName(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int)) return "int";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(long)) return "long";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(double)) return "double";
            if (type == typeof(float)) return "float";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(short)) return "short";
            if (type == typeof(ushort)) return "ushort";

            return null;
        }

        public static string GetPrettyName(this Type type)
        {
            var specialName = GetSpecialName(type);
            
            if (specialName != null) return specialName;
            if (type.IsNested) return type.Name;
            if (type.IsGenericType)
            {
                var sb = new StringBuilder();

                sb.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
                sb.Append("<");

                foreach (var argument in type.GetGenericArguments())
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

        public static string GetPrettyName(this MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                var sb = new StringBuilder();
                var name = method.Name;

                if (name.Contains("`"))
                    name = method.Name.Substring(0, method.Name.IndexOf('`'));

                sb.Append(name);
                sb.Append("<");

                foreach (var argument in method.GetGenericArguments())
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
    }
}