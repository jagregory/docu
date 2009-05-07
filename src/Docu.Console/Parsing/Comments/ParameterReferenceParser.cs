using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    internal class ParameterReferenceParser : CommentParserBase
    {
        public IComment Parse(XmlNode content, bool first, bool last)
        {
            var attribute = content.Attributes["name"];
            var parameterName = attribute == null ? string.Empty : attribute.Value;
            return new ParameterReference(parameterName);
        }
    }
}