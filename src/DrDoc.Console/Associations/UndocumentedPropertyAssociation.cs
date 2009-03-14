using System.Reflection;

namespace DrDoc.Associations
{
    public class UndocumentedPropertyAssociation : PropertyAssociation
    {
        public UndocumentedPropertyAssociation(MemberName name, PropertyInfo property)
            : base(name, null, property)
        {}
    }
}