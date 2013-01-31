using System;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    public class UndocumentedMethod : DocumentedMethod
    {
        public UndocumentedMethod(Identifier name, MethodBase method, Type targetType)
            : base(name, null, method, targetType)
        {
        }
    }
}