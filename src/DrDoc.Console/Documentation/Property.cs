using System.Collections.Generic;
using DrDoc.Documentation.Comments;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class Property : BaseDocumentationElement
    {
        public Property(PropertyIdentifier identifier)
            : base(identifier)
        {
            Summary = new List<IComment>();
            HasGet = identifier.HasGet;
            HasSet = identifier.HasSet;
        }

        public bool HasGet { get; private set; }
        public bool HasSet { get; private set; }

        public IList<IComment> Summary { get; internal set; }
    }
}