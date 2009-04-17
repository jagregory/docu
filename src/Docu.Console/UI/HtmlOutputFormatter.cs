using System.Collections.Generic;
using System.Text;
using System.Web;
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
            MethodUrlFormat = "{type.namespace}/{type}.htm#{method}";
            PropertyUrlFormat = "{type.namespace}/{type}.htm#{property}";
            FieldUrlFormat = "{type.namespace}/{type}.htm#{field}";
            EventUrlFormat = "{type.namespace}/{type}.htm#{event}";
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
            return FormatReferencable(block.Reference);
        }

        public string FormatReferencable(IReferencable reference)
        {
            string url = "";
            string name = reference.PrettyName;

            if (reference is Namespace)
                url = Format(NamespaceUrlFormat, new Replacement("namespace", reference.Name));
            else if (reference is DeclaredType)
                url = Format(TypeUrlFormat,
                             new Replacement("type.namespace", ((DeclaredType)reference).Namespace.Name),
                             new Replacement("type", reference.Name));
            else if (reference is Method)
            {
                var type = ((Method)reference).Type;

                url = Format(MethodUrlFormat,
                             new Replacement("type.namespace", type.Namespace.Name),
                             new Replacement("type", type.Name),
                             new Replacement("method", reference.Name));
            }
            else if (reference is Property)
            {
                var type = ((Property)reference).Type;

                url = Format(PropertyUrlFormat,
                             new Replacement("type.namespace", type.Namespace.Name),
                             new Replacement("type", type.Name),
                             new Replacement("property", reference.Name));
            }
            else if (reference is Field)
            {
                var type = ((Field)reference).Type;

                url = Format(FieldUrlFormat,
                             new Replacement("type.namespace", type.Namespace.Name),
                             new Replacement("type", type.Name),
                             new Replacement("field", reference.Name));
            }
            else if (reference is Event)
            {
                var type = ((Event)reference).Type;

                url = Format(EventUrlFormat,
                             new Replacement("type.namespace", type.Namespace.Name),
                             new Replacement("type", type.Name),
                             new Replacement("event", reference.Name));
            }

            if (reference.IsExternal)
                return "<span title=\"" + reference.FullName + "\">" + Escape(reference.PrettyName) + "</span>";

            return "<a href=\"" + url + "\">" + Escape(name) + "</a>";
        }

        public string Escape(string value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        private string FormatInlineCode(InlineCode block)
        {
            return "<code>" + block.Text + "</code>";
        }

        public string NamespaceUrlFormat { get; set; }
        public string TypeUrlFormat { get; set; }
        public string MethodUrlFormat { get; set; }
        public string PropertyUrlFormat { get; set; }
        public string FieldUrlFormat { get; set; }
        public string EventUrlFormat { get; set; }

        private string Format(string pattern, params Replacement[] replacements)
        {
            string output = pattern;

            foreach (var replacement in replacements)
            {
                output = output.Replace("{" + replacement.Key + "}", replacement.Value);
            }

            return output;
        }

        private class Replacement
        {
            public string Key { get; private set; }
            public string Value { get; private set; }

            public Replacement(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}