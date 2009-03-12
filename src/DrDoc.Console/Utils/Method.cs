using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DrDoc.Utils
{
    public static class Method
    {
        public static MethodInfo Find(Type type, string name, IList<Type> parameters)
        {
            var methodName = name;
            var isGeneric = name.Contains("`");
            var genericArguments = 0;

            if (isGeneric)
            {
                methodName = methodName.Substring(0, methodName.IndexOf('`'));
                genericArguments = Convert.ToInt32(name.Substring(name.LastIndexOf('`') + 1));
            }

            foreach (var method in type.GetMethods())
            {
                if (method.Name == methodName && method.IsGenericMethod == isGeneric)
                {
                    var correctParameters = 0;
                    var methodParams = method.GetParameters();

                    if (methodParams.Length != parameters.Count)
                        continue;

                    for (int i = 0; i < methodParams.Length; i++)
                    {
                        if (parameters[i] == methodParams[i].ParameterType)
                            correctParameters++;
                    }

                    if (correctParameters == parameters.Count && method.GetGenericArguments().Length == genericArguments)
                        return method;
                }
            }

            return null;
        }
    }
}
