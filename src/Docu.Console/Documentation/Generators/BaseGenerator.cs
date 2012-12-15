namespace Docu.Documentation.Generators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using Docu.Documentation.Comments;
    using Docu.Parsing.Comments;
    using Docu.Parsing.Model;

    internal abstract class BaseGenerator
    {
        private readonly ICommentParser commentParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGenerator"/> class.
        /// </summary>
        /// <param name="commentParser">
        /// The comment parser.
        /// </param>
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
                doc.Example =
                    new MultilineCode(
                        this.commentParser.ParseNode(node, new ParseOptions { PreserveWhitespace = true }));
            }
        }

        protected void ParseParamSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null)
            {
                return;
            }

            XmlNode node = member.Xml.SelectSingleNode("param[@name='" + doc.Name + "']");

            this.ParseSummary(node, doc);
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
                doc.Remarks = new Remarks(this.commentParser.ParseNode(node));
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
                doc.Returns = new Summary(this.commentParser.ParseNode(node));
            }
        }

        protected void ParseSummary(IDocumentationMember member, IDocumentationElement doc)
        {
            if (member.Xml == null)
            {
                return;
            }

            XmlNode node = member.Xml.SelectSingleNode("summary");

            this.ParseSummary(node, doc);
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
                doc.Value = new Value(this.commentParser.ParseNode(node));
            }
        }

        private void ParseSummary(XmlNode node, IDocumentationElement doc)
        {
            if (node != null)
            {
                doc.Summary = new Summary(this.commentParser.ParseNode(node));
            }
        }
    }
}