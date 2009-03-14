using System.Reflection;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Associations
{
    public class MethodAssociation : Association
    {
        public MethodAssociation(string name, XmlNode xml, MethodInfo method)
            : base(name, xml)
        {
            Method = method;
        }

        public MethodInfo Method { get; set; }
    }
}