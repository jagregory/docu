using System;
using System.Reflection;
using DrDoc.Parsing.Model;
using DrDoc.Parsing.Model;

namespace DrDoc.Parsing.Model
{
    public class UndocumentedMethod : DocumentedMethod
    {
        public UndocumentedMethod(Identifier name, MethodInfo method, Type targetType)
            : base(name, null, method, targetType)
        {}
    }
}