using System;
using System.Xml;

namespace Docu.Parsing.Model
{
    public interface IDocumentationMember
    {
        XmlNode Xml { get; set; }
        Identifier Name { get; }
        Type TargetType { get; set; }
        bool Match(Identifier name);
    }
}