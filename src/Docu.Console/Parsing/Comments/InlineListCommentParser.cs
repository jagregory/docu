using System;
using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    internal class InlineListCommentParser : CommentParserBase
    {
        private readonly CommentParser _parser;

        public InlineListCommentParser(CommentParser parser)
        {
            _parser = parser;
        }

        public IComment Parse(XmlNode content, bool first, bool last)
        {
            var typeAttribute = content.Attributes["type"];
            var listTypeName = typeAttribute == null ? string.Empty : typeAttribute.Value;
            var list = createListForType(listTypeName);
            foreach (XmlNode itemNode in content.SelectNodes("item"))
            {
                Paragraph term = null;
                Paragraph definition = null;
                var termNode = itemNode.SelectSingleNode("term");
                if (termNode != null)
                {
                    term = new Paragraph(_parser.Parse(termNode.ChildNodes));
                }
                var definitionNode = itemNode.SelectSingleNode("description");
                if (definitionNode != null)
                {
                    definition = new Paragraph(_parser.Parse(definitionNode.ChildNodes));
                }
                list.Items.Add(new InlineListItem(term, definition));
            }
            return list;
        }

        private static InlineList createListForType(string typeName)
        {
            switch(typeName)
            {
                case "definition":
                    return new DefinitionList();
                case "number":
                    return new NumberList();
                case "bullet":
                    return new BulletList();
                case "table":
                    return new TableList();
                default:
                    return new DefinitionList();
            }
        }
    }
}