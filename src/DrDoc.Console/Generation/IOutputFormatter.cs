using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;

namespace DrDoc.Generation
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
