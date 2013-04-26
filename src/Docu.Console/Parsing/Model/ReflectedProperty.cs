using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class ReflectedProperty : DocumentedProperty
    {
        public ReflectedProperty(Identifier name, PropertyInfo property, Type targetType)
            : base(name, null, property, targetType)
        {
        }
    }
}