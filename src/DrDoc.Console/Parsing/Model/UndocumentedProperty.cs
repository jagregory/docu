using System;
using System.Reflection;
using DrDoc.Parsing.Model;

namespace DrDoc.Parsing.Model
{
    public class UndocumentedProperty : DocumentedProperty
    {
        public UndocumentedProperty(Identifier name, PropertyInfo property, Type targetType)
            : base(name, null, property, targetType)
        {}
    }
}