using System.Xml;

namespace DrDoc.Associations
{
    public abstract class Association
    {
        public Association(XmlNode xml)
        {
            Xml = xml;
        }

        public XmlNode Xml { get; set; }
    }
}