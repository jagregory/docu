using System.Reflection;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Associations
{
    public class MethodAssociation : Association
    {
        public MethodAssociation(XmlNode xml, MethodInfo method)
            : base(xml)
        {
            Method = method;
        }

        public MethodInfo Method { get; set; }
    }
}