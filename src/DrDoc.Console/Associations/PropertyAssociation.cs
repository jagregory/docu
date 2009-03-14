using System.Reflection;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Associations
{
    public class PropertyAssociation : Association
    {
        public PropertyAssociation(MemberName name, XmlNode xml, PropertyInfo property)
            : base(name, xml)
        {
            Property = property;
        }

        public PropertyInfo Property { get; set; }
    }
}