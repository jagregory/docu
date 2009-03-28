using System.Collections.Generic;
using System.Text;
using Docu.Documentation;
using Docu.Documentation.Comments;

namespace Docu.UI
{
    public class HtmlOutputFormatter : IOutputFormatter
    {
        private readonly IList<IOutputFormatterPart> Formatters;

        public HtmlOutputFormatter()
        {
            Formatters = new List<IOutputFormatterPart>
            {
                new OutputFormatterPart<Summary>(FormatGeneralContainer),
                new OutputFormatterPart<Remarks>(FormatGeneralContainer),
                new OutputFormatterPart<See>(FormatSee),
                new OutputFormatterPart<InlineText>(FormatInlineText),
                new OutputFormatterPart<InlineCode>(FormatInlineCode),
                new OutputFormatterPart<Paragraph>(FormatParagraph),
            };

            NamespaceUrlFormat = "{namespace}.htm";
            TypeUrlFormat = "{type.namespace}/{type}.htm";
        }

        public string Format(IComment comment)
        {
            foreach (var step in Formatters)
            {
                if (step.Criteria(comment) == true)
                    return step.Action(comment);
            }

            return null;
        }

        private string FormatParagraph(Paragraph comment)
        {
            return "<p>" + FormatGeneralContainer(comment) + "</p>";
        }

        private string FormatGeneralContainer(IComment comment)
        {
            return FormatChildren(comment.Children);
        }

        private string FormatChildren(IEnumerable<IComment> comments)
        {
            var sb = new StringBuilder();

            foreach (var comment in comments)
            {
                sb.Append(Format(comment));
            }

            return sb.ToString();
        }

        private string FormatInlineText(InlineText comment)
        {
            return comment.Text;
        }

        private string FormatSee(See block)
        {
            string url = "";

            if (block.Reference is Namespace)
                url = Format(NamespaceUrlFormat,
                             new Dictionary<string, string> { { "namespace", block.Reference.Name } });
            else if (block.Reference is DeclaredType)
                url = Format(TypeUrlFormat,
                             new Dictionary<string, string>
                             {
                                 { "type.namespace", ((DeclaredType)block.Reference).Namespace.Name },
                                 { "type", block.Reference.Name }
                             });

            return "<a href=\"" + url + "\">" + block.Reference.Name + "</a>";
        }

        public string FormatReferencable(IReferencable reference)
        {
            string url = "";
            string name = reference.PrettyName;

            if (reference is Namespace)
                url = Format(NamespaceUrlFormat, new Dictionary<string, string> { { "namespace", reference.Name } });
            else if (reference is DeclaredType)
                url = Format(TypeUrlFormat,
                             new Dictionary<string, string>
                             {
                                 { "type.namespace", ((DeclaredType)reference).Namespace.Name },
                                 { "type", reference.Name }
                             });

            if (reference.IsExternal)
                return "<span title=\"" + reference.FullName + "\">" + Escape(reference.PrettyName) + "</span>";

            return "<a href=\"" + url + "\">" + Escape(name) + "</a>";
        }

        public string Escape(string value)
        {
            return value.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private string FormatInlineCode(InlineCode block)
        {
            return "<code>" + block.Text + "</code>";
        }

        public string NamespaceUrlFormat { get; set; }
        public string TypeUrlFormat { get; set; }

        private string Format(string pattern, IDictionary<string, string> pairs)
        {
            string output = pattern;

            foreach (var pair in pairs)
            {
                output = output.Replace("{" + pair.Key + "}", pair.Value);
            }

            return output;
        }
    }
}