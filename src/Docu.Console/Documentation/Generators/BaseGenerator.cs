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
        private readonly ICommentParser commentParser;

        protected BaseGenerator(ICommentParser commentParser)
        {
            this.commentParser = commentParser;
        }

        private void ParseSummary(XmlNode node, IDocumentationElement doc)
        {
            if (node != null)
                doc.Summary = new Summary(commentParser.ParseNode(node));
        }

        protected void ParseParamSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;

            var node = member.Xml.SelectSingleNode("param[@name='" + doc.Name + "']");

            ParseSummary(node, doc);
        }

        protected void ParseValue(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;

            var node = member.Xml.SelectSingleNode("value");

            if (node != null)
                doc.Value = new Value(commentParser.ParseNode(node));
        }

        protected void ParseSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;

            var node = member.Xml.SelectSingleNode("summary");

            ParseSummary(node, doc);
        }

        protected void ParseRemarks(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;

            var node = member.Xml.SelectSingleNode("remarks");

            if (node != null)
                doc.Remarks = new Remarks(commentParser.ParseNode(node));
        }

        protected void ParseExample(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null) return;

            var node = member.Xml.SelectSingleNode("example");

            if (node != null)
                doc.Example = new MultilineCode(commentParser.ParseNode(node, new ParseOptions { PreserveWhitespace = true }));
        }

        protected void ParseReturns(IDocumentationMember member, Method doc)
        {
            if (member.Xml == null) return;

            var node = member.Xml.SelectSingleNode("returns");

            if (node != null)
                doc.Returns = new Summary(commentParser.ParseNode(node));
        }


        protected Namespace FindNamespace(IDocumentationMember association, List<Namespace> namespaces)
        {
            var identifier = Identifier.FromNamespace(association.TargetType.Namespace);
            Namespace ns=  namespaces.Find(x => x.IsIdentifiedBy(identifier));
            if (ns == null)
            {
                for (int i = 0; i < namespaces.Count; i++)
                {
                    if (namespaces[i].Name == "NoNamespace")
                    {
                        ns = namespaces[i];
                        break;
                    }
                }
            }
            return ns;
        }

        protected DeclaredType FindType(Namespace ns, IDocumentationMember association)
        {
            var typeName = Identifier.FromType(association.TargetType);
            return ns.Types.FirstOrDefault(x => x.IsIdentifiedBy(typeName));
        }
    }
}