using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Parsing
{
    public class CommentContentParser : ICommentContentParser
    {
        private readonly IDictionary<Func<XmlNode, bool>, Func<XmlNode, DocBlock>> parsers = new Dictionary<Func<XmlNode, bool>, Func<XmlNode, DocBlock>>();
        
        public CommentContentParser()
        {
            parsers.Add(node => node is XmlText, ParseText);
            parsers.Add(node => node.Name == "c", ParseInlineCode);
            parsers.Add(node => node.Name == "code", ParseMultilineCode);
            parsers.Add(node => node.Name == "see", ParseSee);
            parsers.Add(node => node.Name == "para", ParseParagraph);
        }

        public IList<DocBlock> Parse(XmlNode content)
        {
            var blocks = new List<DocBlock>();

            foreach (XmlNode node in content.ChildNodes)
            {
                foreach (var pair in parsers)
                {
                    var isValid = pair.Key;
                    var parser = pair.Value;

                    if (isValid(node))
                    {
                        var block = parser(node);

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

        private DocBlock ParseText(XmlNode content)
        {
            return new DocTextBlock(PrepareText(content.InnerText));
        }

        private DocBlock ParseInlineCode(XmlNode content)
        {
            return new DocCodeBlock(PrepareText(content.InnerText));
        }

        private DocBlock ParseMultilineCode(XmlNode content)
        {
            return new DocCodeBlock(PrepareText(content.InnerText));
        }

        private DocBlock ParseSee(XmlNode content)
        {
            var referenceTarget = MemberName.FromString(content.Attributes["cref"].Value);

            return new DocReferenceBlock(new UnresolvedReference(referenceTarget));
        }

        private DocBlock ParseParagraph(XmlNode content)
        {
            return new DocParagraphBlock(PrepareText(content.InnerText));
        }

        private string PrepareText(string text)
        {
            var regexp = new Regex(@"[\s]{0,}\r\n[\s]{0,}");
            var prepared = text.Trim(); // remove leading and trailing whitespace

            prepared = regexp.Replace(prepared, "\r\n");
            
            return prepared;
        }
    }
}
