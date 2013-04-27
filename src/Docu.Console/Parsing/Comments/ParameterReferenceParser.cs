using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public class ParameterReferenceParser : ICommentNodeParser
    {
        public bool CanParse(XmlNode node)
        {
            return node.Name == "paramref";
        }

        public Comment Parse(ICommentParser parser, XmlNode node, bool first, bool last, ParseOptions options)
        {
            var attribute = node.Attributes["name"];
            var parameterName = attribute == null ? string.Empty : attribute.Value;
            return new ParameterReference(parameterName);
        }
    }
}