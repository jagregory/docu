using Docu.Documentation;
using Docu.Documentation.Comments;

namespace Docu.UI
{
    public interface IOutputFormatter
    {
        string NamespaceUrlFormat { get; set; }
        string TypeUrlFormat { get; set; }
        string Format(See block);
        string Format(InlineCode block);
        string Format(IReferencable reference);
        string Escape(string value);
    }
}