using System.Reflection;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Associations
{
    public class PropertyAssociation : Association
    {
        public PropertyAssociation(XmlNode xml, PropertyInfo property)
            : base(xml)
        {
            Property = property;
        }

        public PropertyInfo Property { get; set; }
    }
}