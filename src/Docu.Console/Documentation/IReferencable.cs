using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IReferencable : IResolvable
    {
        string Name { get; }
        string FullName { get; }
        string PrettyName { get; }
        bool IsExternal { get; }
        bool HasDocumentation { get; }
        Summary Summary { get; set; }
        Remarks Remarks { get; set; }
        Value Value { get; set; }
        bool IsIdentifiedBy(Identifier otherIdentifier);
        void ConvertToExternalReference();
    }
}
