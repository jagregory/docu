using System;
using System.Xml;

namespace Docu.Parsing.Model
{
    public interface IDocumentationMember
    {
        XmlNode Xml { get; }
        Identifier Name { get; }
        Type TargetType { get; }
    }
}