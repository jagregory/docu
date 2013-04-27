using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public class InlineListCommentParser : ICommentNodeParser
    {
        public bool CanParse(XmlNode node)
        {
            return node.Name == "list";
        }

        public Comment Parse(ICommentParser parser, XmlNode node, bool first, bool last, ParseOptions options)
        {
            var typeAttribute = node.Attributes["type"];
            var listTypeName = typeAttribute == null ? string.Empty : typeAttribute.Value;
            var list = createListForType(listTypeName);
            foreach (XmlNode itemNode in node.SelectNodes("item"))
            {
                Paragraph term = null;
                Paragraph definition = null;
                var termNode = itemNode.SelectSingleNode("term");
                if (termNode != null)
                {
                    term = new Paragraph(parser.Parse(termNode.ChildNodes));
                }
                var definitionNode = itemNode.SelectSingleNode("description");
                if (definitionNode != null)
                {
                    definition = new Paragraph(parser.Parse(definitionNode.ChildNodes));
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