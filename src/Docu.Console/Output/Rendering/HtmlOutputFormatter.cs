using Docu.Documentation;
using Docu.Documentation.Comments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Docu.Output.Rendering
{
    public class HtmlOutputFormatter
    {
        readonly IList<IOutputFormatterPart> Formatters;
        readonly IDocuTemplate view;

        public HtmlOutputFormatter(IDocuTemplate view)
        {
            this.view = view;

            Formatters = new List<IOutputFormatterPart>
                {
                    new OutputFormatterPart<Summary>(FormatGeneralContainer),
                    new OutputFormatterPart<Remarks>(FormatGeneralContainer),
                    new OutputFormatterPart<See>(FormatSee),
                    new OutputFormatterPart<InlineText>(FormatInlineText),
                    new OutputFormatterPart<InlineCode>(FormatInlineCode),
                    new OutputFormatterPart<MultilineCode>(FormatMultilineCode),
                    new OutputFormatterPart<Paragraph>(FormatParagraph),
                    new OutputFormatterPart<ParameterReference>(FormatParameterReference),
                    new OutputFormatterPart<DefinitionList>(FormatDefinitionList),
                    new OutputFormatterPart<BulletList>(FormatBulletList),
                    new OutputFormatterPart<NumberList>(FormatNumberList),
                    new OutputFormatterPart<TableList>(FormatTableList),
                };

            NamespaceUrlFormat = "~/{namespace}/index.htm";
            TypeUrlFormat = "~/{type.namespace}/{type}.htm";
            MethodUrlFormat = "~/{type.namespace}/{type}.htm#{method}";
            PropertyUrlFormat = "~/{type.namespace}/{type}.htm#{property}";
            FieldUrlFormat = "~/{type.namespace}/{type}.htm#{field}";
            EventUrlFormat = "~/{type.namespace}/{type}.htm#{event}";
        }

        public string Format(Comment comment)
        {
            foreach (var step in Formatters)
            {
                if (step.Criteria(comment))
                    return step.Action(comment);
            }

            return null;
        }

        string FormatParagraph(Paragraph comment)
        {
            return "<p>" + FormatGeneralContainer(comment) + "</p>";
        }

        string FormatGeneralContainer(Comment comment)
        {
            return FormatChildren(comment.Children);
        }

        string FormatChildren(IEnumerable<Comment> comments)
        {
            var sb = new StringBuilder();

            foreach (var comment in comments)
            {
                sb.Append(Format(comment) + " ");
            }

            sb.Replace(" .", ".");

            return sb.ToString();
        }

        string FormatInlineText(InlineText comment)
        {
            return comment.Text;
        }

        string FormatParameterReference(ParameterReference comment)
        {
            return "<var>" + comment.Parameter + "</var>";
        }

        string formatInlineList(IEnumerable<InlineListItem> items, string outerTagFormat, string itemFormat, string termFormat, string definitionFormat)
        {
            var output = new StringBuilder();
            foreach (var listItem in items)
            {
                string term = null;
                string definition = null;
                if (listItem.Term != null) term = String.Format(termFormat, FormatGeneralContainer(listItem.Term));
                if (listItem.Definition != null) definition = String.Format(definitionFormat, FormatGeneralContainer(listItem.Definition));
                output.AppendFormat(itemFormat, term, definition);
            }
            return string.Format(outerTagFormat, output);
        }

        string FormatDefinitionList(DefinitionList comment)
        {
            return formatInlineList(comment.Items, "<dl>{0}</dl>", "{0}{1}", "<dt>{0}</dt>", "<dd>{0}</dd>");
        }

        string FormatTableList(TableList comment)
        {
            return formatInlineList(comment.Items, "<table>{0}</table>", "<tr>{0}{1}</tr>", "<td>{0}</td>", "<td>{0}</td>");
        }

        string FormatNumberList(NumberList comment)
        {
            return formatInlineList(comment.Items, "<ol>{0}</ol>", "<li>{0}{1}</li>", "{0}", "{0}");
        }

        string FormatBulletList(BulletList comment)
        {
            return formatInlineList(comment.Items, "<ul>{0}</ul>", "<li>{0}{1}</li>", "{0}", "{0}");
        }

        string FormatSee(See block)
        {
            return FormatReferencable(block.Reference);
        }

        public string FormatReferencable(IReferencable reference)
        {
            return FormatReferencable(reference, new KeyValuePair<string, string>[0]);
        }

        public string FormatReferencable(IReferencable reference, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            if (reference == null)
            {
                return string.Empty;
            }

            string url = "";
            string name = reference.PrettyName;

            if (reference is Namespace)
                url = Format(NamespaceUrlFormat, new Replacement("namespace", reference.Name));
            else if (reference is DeclaredType)
                url = Format(TypeUrlFormat,
                    new Replacement("type.namespace", ((DeclaredType) reference).Namespace.Name),
                    new Replacement("type", reference.Name));
            else if (reference is Method)
            {
                var type = ((Method) reference).Type;

                url = Format(MethodUrlFormat,
                    new Replacement("type.namespace", type.Namespace.Name),
                    new Replacement("type", type.Name),
                    new Replacement("method", reference.Name));
            }
            else if (reference is Property)
            {
                var type = ((Property) reference).Type;

                url = Format(PropertyUrlFormat,
                    new Replacement("type.namespace", type.Namespace.Name),
                    new Replacement("type", type.Name),
                    new Replacement("property", reference.Name));
            }
            else if (reference is Field)
            {
                var type = ((Field) reference).Type;

                url = Format(FieldUrlFormat,
                    new Replacement("type.namespace", type.Namespace.Name),
                    new Replacement("type", type.Name),
                    new Replacement("field", reference.Name));
            }
            else if (reference is Event)
            {
                var type = ((Event) reference).Type;

                url = Format(EventUrlFormat,
                    new Replacement("type.namespace", type.Namespace.Name),
                    new Replacement("type", type.Name),
                    new Replacement("event", reference.Name));
            }

            if (reference.IsExternal)
                return "<span title=\"" + reference.FullName + "\">" + Escape(reference.PrettyName) + "</span>";

            var attributeHtml = "";

            attributes.ForEach(x => attributeHtml += " " + x.Key + "=\"" + x.Value + "\"");

            return "<a href=\"" + view.SiteResource(url) + "\"" + attributeHtml + ">" + Escape(name) + "</a>";
        }

        public string Escape(string value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        string FormatInlineCode(InlineCode block)
        {
            return "<code>" + block.Text + "</code>";
        }

        string FormatMultilineCode(MultilineCode block)
        {
            return FormatGeneralContainer(block);
        }

        public string NamespaceUrlFormat { get; set; }
        public string TypeUrlFormat { get; set; }
        public string MethodUrlFormat { get; set; }
        public string PropertyUrlFormat { get; set; }
        public string FieldUrlFormat { get; set; }
        public string EventUrlFormat { get; set; }

        string Format(string pattern, params Replacement[] replacements)
        {
            string output = pattern;

            foreach (var replacement in replacements)
            {
                output = output.Replace("{" + replacement.Key + "}", replacement.Value);
            }

            return output;
        }

        class Replacement
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