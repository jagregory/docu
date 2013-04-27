using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal abstract class BaseGenerator
    {
        readonly ICommentParser _commentParser;

        protected BaseGenerator(ICommentParser commentParser)
        {
            _commentParser = commentParser;
        }

        protected static Namespace FindNamespace(IDocumentationMember association, List<Namespace> namespaces)
        {
            NamespaceIdentifier identifier = IdentifierFor.Namespace(association.TargetType.Namespace);
            return namespaces.Find(x => x.IsIdentifiedBy(identifier));
        }

        protected static DeclaredType FindType(IDocumentationMember association, List<Namespace> namespaces)
        {
            var typeName = IdentifierFor.Type(association.TargetType);
            var identifier = IdentifierFor.Namespace(association.TargetType.Namespace);
            return namespaces.Find(x => x.IsIdentifiedBy(identifier)).Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));
        }

        protected void ParseExample(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;
            XmlNode node = member.Xml.SelectSingleNode("example");
            if (node == null) return;

            doc.Example = new MultilineCode(_commentParser.ParseNode(node, new ParseOptions {PreserveWhitespace = true}));
        }

        protected void ParseParamSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;
            XmlNode node = member.Xml.SelectSingleNode("param[@name='" + doc.Name + "']");
            if (node == null) return;

            doc.Summary = new Summary(_commentParser.ParseNode(node));
        }

        protected void ParseRemarks(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;
            XmlNode node = member.Xml.SelectSingleNode("remarks");
            if (node == null) return;

            doc.Remarks = new Remarks(_commentParser.ParseNode(node));
        }

        protected void ParseReturns(IDocumentationMember member, Method doc)
        {
            if (member.Xml == null) return;
            XmlNode node = member.Xml.SelectSingleNode("returns");
            if (node == null) return;

            doc.Returns = new Summary(_commentParser.ParseNode(node));
        }

        protected void ParseSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;
            XmlNode node = member.Xml.SelectSingleNode("summary");
            if (node == null) return;

            doc.Summary = new Summary(_commentParser.ParseNode(node));
        }

        protected void ParseValue(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;
            XmlNode node = member.Xml.SelectSingleNode("value");
            if (node == null) return;

            doc.Value = new Value(_commentParser.ParseNode(node));
        }
    }
}
