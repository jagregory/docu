using System.Reflection;
using DrDoc.Model;

namespace DrDoc.Model
{
    public class UndocumentedProperty : DocumentedProperty
    {
        public UndocumentedProperty(Identifier name, PropertyInfo property)
            : base(name, null, property)
        {}
    }
}