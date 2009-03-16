using System.Collections.Generic;
using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing
{
    public interface ICommentContentParser
    {
        IList<IComment> Parse(XmlNode content);
    }
}