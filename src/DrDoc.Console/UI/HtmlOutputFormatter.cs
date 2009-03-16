using System.Collections.Generic;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;

namespace DrDoc.UI
{
    internal class HtmlOutputFormatter : IOutputFormatter
    {
        public HtmlOutputFormatter()
        {
            NamespaceUrlFormat = "{namespace}.htm";
            TypeUrlFormat = "{type.namespace}/{type}.htm";
        }

        public string Format(See block)
        {
            var url = "";

            if (block.Reference is Namespace)
                url = Format(NamespaceUrlFormat, new Dictionary<string, string> {{"namespace", block.Reference.Name}});
            else if (block.Reference is DeclaredType)
                url = Format(TypeUrlFormat, new Dictionary<string, string> {{"type.namespace", ((DeclaredType)block.Reference).Namespace.Name}, {"type", block.Reference.Name.ToString()}});

            return "<a href=\"" + url + "\">" + block.Reference.Name + "</a>";
        }

        public string Format(IReferencable reference)
        {
            var url = "";
            var name = reference.PrettyName;

            if (reference is Namespace)
                url = Format(NamespaceUrlFormat, new Dictionary<string, string> {{"namespace", reference.Name}});
            else if (reference is DeclaredType)
                url = Format(TypeUrlFormat, new Dictionary<string, string> { { "type.namespace", ((DeclaredType)reference).Namespace.Name }, { "type", reference.Name } });

            if (reference.IsExternal)
                return "<span title=\"" + reference.FullName + "\">" + Escape(reference.PrettyName) + "</span>";

            return "<a href=\"" + url + "\">" + Escape(name) + "</a>";
        }

        public string Escape(string value)
        {
            return value.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public string Format(InlineCode block)
        {
            return "<code>" + block.Text + "</code>";
        }

        private string Format(string pattern, IDictionary<string, string> pairs)
        {
            var output = pattern;

            foreach (var pair in pairs)
            {
                output = output.Replace("{" + pair.Key + "}", pair.Value);
            }

            return output;
        }

        public string NamespaceUrlFormat { get; set; }
        public string TypeUrlFormat { get; set; }
    }
}