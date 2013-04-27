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

        public IList<Comment> Parse(XmlNodeList nodes)
        {
            return Parse(nodes, new ParseOptions());
        }

        public IList<Comment> Parse(XmlNodeList nodes, ParseOptions options)
        {
            var blocks = new List<Comment>();

            var count = nodes.Count;
            for(var i = 0; i < count; i++)
            {
                var node = nodes[i];
                var first = (i == 0);
                var last = (i == (count - 1));

                var parser = _parsers.FirstOrDefault(p => p.CanParse(node));
                if (parser == null) continue;
                
                var block = parser.Parse(this, node, first, last, options);
                if (block != null)
                {
                    blocks.Add(block);
                }
            }

            return blocks;
        }

        public IList<Comment> ParseNode(XmlNode node)
        {
            return Parse(node.ChildNodes, new ParseOptions());
        }

        public IList<Comment> ParseNode(XmlNode node, ParseOptions options)
        {
            return Parse(node.ChildNodes, options);
        }
    }
}