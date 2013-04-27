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
        readonly ICommentParser commentParser;

        protected BaseGenerator(ICommentParser commentParser)
        {
            this.commentParser = commentParser;
        }

        protected Namespace FindNamespace(IDocumentationMember association, List<Namespace> namespaces)
        {
            NamespaceIdentifier identifier = Identifier.FromNamespace(association.TargetType.Namespace);
            return namespaces.Find(x => x.IsIdentifiedBy(identifier));
        }

        protected DeclaredType FindType(Namespace ns, IDocumentationMember association)
        {
            TypeIdentifier typeName = Identifier.FromType(association.TargetType);
            return ns.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));
        }

        protected void ParseExample(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null)
            {
                return;
            }

            XmlNode node = member.Xml.SelectSingleNode("example");

            if (node != null)
            {
                doc.Example = new MultilineCode(
                    commentParser.ParseNode(node, new ParseOptions {PreserveWhitespace = true}));
            }
        }

        protected void ParseParamSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null)
            {
                return;
            }

            XmlNode node = member.Xml.SelectSingleNode("param[@name='" + doc.Name + "']");

            ParseSummary(node, doc);
        }

        protected void ParseRemarks(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null)
            {
                return;
            }

            XmlNode node = member.Xml.SelectSingleNode("remarks");

            if (node != null)
            {
                doc.Remarks = new Remarks(commentParser.ParseNode(node));
            }
        }

        protected void ParseReturns(IDocumentationMember member, Method doc)
        {
            if (member.Xml == null)
            {
                return;
            }

            XmlNode node = member.Xml.SelectSingleNode("returns");

            if (node != null)
            {
                doc.Returns = new Summary(commentParser.ParseNode(node));
            }
        }

        protected void ParseSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null)
            {
                return;
            }

            XmlNode node = member.Xml.SelectSingleNode("summary");

            ParseSummary(node, doc);
        }

        protected void ParseValue(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null)
            {
                return;
            }

            XmlNode node = member.Xml.SelectSingleNode("value");

            if (node != null)
            {
                doc.Value = new Value(commentParser.ParseNode(node));
            }
        }

        void ParseSummary(XmlNode node, IDocumentationElement doc)
        {
            if (node != null)
            {
                doc.Summary = new Summary(commentParser.ParseNode(node));
            }
        }
    }
}
