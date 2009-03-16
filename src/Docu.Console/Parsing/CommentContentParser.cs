using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public class CommentContentParser : ICommentContentParser
    {
        private readonly IDictionary<Func<XmlNode, bool>, Func<XmlNode, IComment>> parsers =
            new Dictionary<Func<XmlNode, bool>, Func<XmlNode, IComment>>();

        public CommentContentParser()
        {
            parsers.Add(node => node is XmlText, ParseText);
            parsers.Add(node => node.Name == "c", ParseInlineCode);
            parsers.Add(node => node.Name == "code", ParseMultilineCode);
            parsers.Add(node => node.Name == "see", ParseSee);
            parsers.Add(node => node.Name == "para", ParseParagraph);
        }

        public IList<IComment> Parse(XmlNode content)
        {
            var blocks = new List<IComment>();

            foreach (XmlNode node in content.ChildNodes)
            {
                foreach (var pair in parsers)
                {
                    Func<XmlNode, bool> isValid = pair.Key;
                    Func<XmlNode, IComment> parser = pair.Value;

                    if (isValid(node))
                    {
                        IComment block = parser(node);

                        if (block != null)
                        {
                            blocks.Add(block);
                            continue;
                        }
                    }
                }
            }

            return blocks.AsReadOnly();
        }

        private IComment ParseText(XmlNode content)
        {
            return new InlineText(PrepareText(content.InnerText));
        }

        private IComment ParseInlineCode(XmlNode content)
        {
            return new InlineCode(PrepareText(content.InnerText));
        }

        private IComment ParseMultilineCode(XmlNode content)
        {
            return new InlineCode(PrepareText(content.InnerText));
        }

        private IComment ParseSee(XmlNode content)
        {
            Identifier referenceTarget = Identifier.FromString(content.Attributes["cref"].Value);
            IReferencable reference = null;

            if (referenceTarget is TypeIdentifier)
                reference = DeclaredType.Unresolved((TypeIdentifier)referenceTarget,
                                                    Namespace.Unresolved(referenceTarget.CloneAsNamespace()));
            else if (referenceTarget is MethodIdentifier)
                reference = Method.Unresolved((MethodIdentifier)referenceTarget);

            return new See(reference);
        }

        private IComment ParseParagraph(XmlNode content)
        {
            return new Paragraph(PrepareText(content.InnerText));
        }

        private string PrepareText(string text)
        {
            var regexp = new Regex(@"[\s]{0,}\r\n[\s]{0,}");
            string prepared = text.Trim(); // remove leading and trailing whitespace

            prepared = regexp.Replace(prepared, "\r\n");

            return prepared;
        }
    }
}