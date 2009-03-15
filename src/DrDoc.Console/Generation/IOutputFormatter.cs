using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrDoc.Generation
{
    public interface IOutputFormatter
    {
        string Format(DocReferenceBlock block);
        string Format(DocCodeBlock block);
        string Format(IReferencable reference);
        string Escape(string value);

        string NamespaceUrlFormat { get; set; }
        string TypeUrlFormat { get; set; }
    }
}
