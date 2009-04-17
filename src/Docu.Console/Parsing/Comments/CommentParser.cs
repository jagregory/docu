using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public class CommentParser : ICommentParser
    {
        private readonly IDictionary<Func<XmlNode, bool>, Func<XmlNode, bool, bool, IComment>> parsers =
            new Dictionary<Func<XmlNode, bool>, Func<XmlNode, bool, bool, IComment>>();

        private readonly InlineTextCommentParser InlineText = new InlineTextCommentParser();
        private readonly InlineCodeCommentParser InlineCode = new InlineCodeCommentParser();
        private readonly MultilineCodeCommentParser MultilineCode = new MultilineCodeCommentParser();
        private readonly SeeCodeCommentParser See = new SeeCodeCommentParser();
        private readonly ParagraphCommentParser Paragraph;

        public CommentParser()
        {
            Paragraph = new ParagraphCommentParser(this);

            parsers.Add(node => node is XmlText, InlineText.Parse);
            parsers.Add(node => node.Name == "c", InlineCode.Parse);
            parsers.Add(node => node.Name == "code", MultilineCode.Parse);
            parsers.Add(node => node.Name == "see", See.Parse);
            parsers.Add(node => node.Name == "para", Paragraph.Parse);
        }

        public IList<IComment> Parse(XmlNodeList nodes)
        {
            var blocks = new List<IComment>();

            int count = nodes.Count;
            for(int i = 0; i < count; i++)
            {
                XmlNode node = nodes[i];
                bool first = (i == 0);
                bool last = (i == count);

                foreach(var pair in parsers)
                {
                    var isValid = pair.Key;
                    var parser = pair.Value;

                    if(!isValid(node))
                        continue;

                    var block = parser(node, first, last);

                    if(block != null)
                    {
                        blocks.Add(block);
                        continue;
                    }
                }
            }

            return blocks;
        }

        public IList<IComment> Parse(XmlNode content)
        {
            var blocks = new List<IComment>();

            blocks.AddRange(Parse(content.ChildNodes));

            return blocks.AsReadOnly();
        }
    }
}