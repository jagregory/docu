using System.Collections.Generic;

namespace DrDoc.Generation
{
    internal class HtmlOutputFormatter : IOutputFormatter
    {
        public HtmlOutputFormatter()
        {
            NamespaceUrlFormat = "{namespace}.htm";
            TypeUrlFormat = "{type.namespace}/{type}.htm";
        }

        public string Format(DocReferenceBlock block)
        {
            var url = "";

            if (block.Reference is DocNamespace)
                url = Format(NamespaceUrlFormat, new Dictionary<string, string> {{"namespace", block.Reference.Name.ToString()}});
            else if (block.Reference is DocType)
                url = Format(TypeUrlFormat, new Dictionary<string, string> {{"type.namespace", ((DocType)block.Reference).Namespace.Name.ToString()}, {"type", block.Reference.Name.ToString()}});

			return "<a href=\"" + url + "\">" + block.Reference.Name + "</a>";
        }

        public string Format(IReferencable reference)
        {
            if (reference is ExternalReference)
                return Format((ExternalReference)reference);
            
            var url = "";

            if (reference is DocNamespace)
                url = Format(NamespaceUrlFormat, new Dictionary<string, string> {{"namespace", reference.Name.ToString()}});
            else if (reference is DocType)
                url = Format(TypeUrlFormat, new Dictionary<string, string> {{"type.namespace", ((DocType)reference).Namespace.Name.ToString()}, {"type", reference.Name.ToString()}});

            return "<a href=\"" + url + "\">" + reference.Name + "</a>";
        }

        public string Format(ExternalReference reference)
        {
            return "<span title=\"" + reference.FullName + "\">" + reference.Name + "</span>";
        }

        public string Format(DocCodeBlock block)
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