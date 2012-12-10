using System.Linq;
using Docu.Documentation;

namespace Docu.TemplateExtensions
{
    public static class TemplateHelperExtensions
    {
        public static bool IsDocumented(this IDocumentationElement documentationElement)
        {
            return documentationElement.Summary.Children.Any();
        }
    }
}