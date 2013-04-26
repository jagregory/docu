using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class ReflectedMethod : DocumentedMethod
    {
        public ReflectedMethod(Identifier name, MethodBase method, Type targetType)
            : base(name, null, method, targetType)
        {
        }
    }
}