using System.Collections.Generic;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IDocumentationElement
    {
        string Name { get; }
        bool IsExternal { get; }
        bool IsResolved { get; }
        IList<IComment> Summary { get; set; }
        IList<IComment> Remarks { get; set; }
        IList<IComment> Value { get; set; }
        bool IsIdentifiedBy(Identifier otherIdentifier);
        void ConvertToExternalReference();
    }
}