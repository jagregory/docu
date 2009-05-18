using System.Collections.Generic;
using System.Xml;
using Docu.Documentation.Comments;
using System.Linq;

namespace Docu.Parsing.Comments
{
    public class CommentParser : ICommentParser
    {
        private readonly ICommentNodeParser[] _parsers;

        public CommentParser(ICommentNodeParser[] parsers)
        {
            _parsers = parsers;
        }

        public IList<IComment> Parse(XmlNodeList nodes)
        {
            var blocks = new List<IComment>();

            var count = nodes.Count;
            for(var i = 0; i < count; i++)
            {
                var node = nodes[i];
                var first = (i == 0);
                var last = (i == (count - 1));

                var parser = _parsers.FirstOrDefault(p => p.CanParse(node));
                if (parser == null) continue;
                
                var block = parser.Parse(this, node, first, last);
                if (block != null)
                {
                    blocks.Add(block);
                }
            }

            return blocks;
        }

        public IList<IComment> ParseNode(XmlNode node)
        {
            var blocks = new List<IComment>();

            blocks.AddRange(Parse(node.ChildNodes));

            return blocks.AsReadOnly();
        }
    }
}