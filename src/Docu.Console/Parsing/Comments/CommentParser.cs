using System;
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
            return Parse(nodes, new ParseOptions());
        }

        public IList<IComment> Parse(XmlNodeList nodes, ParseOptions options)
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
                
                var block = parser.Parse(this, node, first, last, options);
                if (block != null)
                {
                    blocks.Add(block);
                }
            }

            return blocks;
        }

        public IList<IComment> ParseNode(XmlNode node)
        {
            return ParseNode(node, new ParseOptions());
        }

        public IList<IComment> ParseNode(XmlNode node, ParseOptions options)
        {
            var blocks = new List<IComment>();

            blocks.AddRange(Parse(node.ChildNodes, options));

            return blocks.AsReadOnly();
        }
    }
}