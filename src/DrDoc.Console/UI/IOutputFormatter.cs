using DrDoc.Documentation;
using DrDoc.Documentation.Comments;

namespace DrDoc.UI
{
    public interface IOutputFormatter
    {
        string Format(See block);
        string Format(InlineCode block);
        string Format(IReferencable reference);
        string Escape(string value);

        string NamespaceUrlFormat { get; set; }
        string TypeUrlFormat { get; set; }
    }
}