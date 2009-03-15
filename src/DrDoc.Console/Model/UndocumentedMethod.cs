using System;
using System.Reflection;
using DrDoc.Model;

namespace DrDoc.Model
{
    public class UndocumentedMethod : DocumentedMethod
    {
        public UndocumentedMethod(Identifier name, MethodInfo method, Type targetType)
            : base(name, null, method, targetType)
        {}
    }
}