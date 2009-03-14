namespace DrDoc.Generation
{
    internal class HtmlOutputFormatter : IOutputFormatter
    {
        public string Format(DocReferenceBlock block)
        {
            var url = "";

            if (block.Reference is DocNamespace)
                url = block.Reference.Name + ".htm";
            else if (block.Reference is DocType)
                url = ((DocType)block.Reference).Namespace.Name + "/" + block.Reference.Name + ".htm";

			return "<a href=\"" + url + "\">" + block.Reference.Name + "</a>";
        }

        public string Format(DocCodeBlock block)
        {
            return "<code>" + block.Text + "</code>";
        }
    }
}