using System.Collections.Generic;
using System.Xml;

namespace DrDoc.Parsing
{
    public interface ICommentContentParser
    {
        IList<DocBlock> Parse(XmlNode content);
    }
}