using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Generation;
using Spark;

namespace DrDoc.UI
{
    /// <summary>
    /// Base class of all spark views. In this example it's named in the
    /// web.config spark/pages/@pageBaseType attribute. 
    /// 
    /// If you use #latebound syntax in expressions you need to have Eval
    /// methods in the base class, and with direct usage it's a
    /// "bring your own Eval" situation.
    /// 
    /// For convenience this example will rely on the ViewDataDictionary.
    /// </summary>
    public abstract class SparkTemplateBase : AbstractSparkView
    {
        protected readonly IOutputFormatter Formatter = new HtmlOutputFormatter();

        public string h(string content)
        {
            return Formatter.Escape(content);
        }

        public string WriteReference(IReferencable reference)
        {
            return Formatter.Format(reference);
        }

        public string WriteSummary(IList<IComment> summary)
        {
            var sb = new StringBuilder();

            foreach (var block in summary)
            {
                if (block is InlineCode)
                {
                    sb.Append(Formatter.Format((InlineCode)block));
                    sb.Append(" ");
                    continue;
                }
                if (block is IReferrer)
                {
                    sb.Append(WriteReference(((IReferrer)block).Reference));
                    sb.Append(" ");
                    continue;
                }

                sb.Append(block.ToString());
                sb.Append(" ");
            }

            if (sb.Length > 0)
                sb.Length--; // trim trailing whitespace

            return sb.ToString();
        }

        public string OutputMethodParams(Method method)
        {
            var sb = new StringBuilder();

            foreach (var parameter in method.Parameters)
            {
                sb.Append(parameter.Reference.Name);
                sb.Append(" ");
                sb.Append(parameter.Name);
                sb.Append(", ");
            }

            if (sb.Length > 0)
                sb.Length -= 2;

            return sb.ToString();
        }

        public object Eval(string expression)
        {
            //            return ViewData.Eval(expression);
            return null;
        }

        public string Eval(string expression, string format)
        {
            //return ViewData.Eval(expression, format);
            return null;
        }

        /// <summary>
        /// Members of this class are also available to the views
        /// </summary>
        public bool IsInStock(int productId)
        {
            return DateTime.UtcNow.Second % 2 == 1;
        }

        public IList<Namespace> Namespaces
        {
            get { return ViewData.Namespaces; }
        }

        public Namespace Namespace
        {
            get { return ViewData.Namespace; }
        }

        public DeclaredType Type
        {
            get { return ViewData.Type; }
        }

        public OutputData ViewData { get; set; }
    }
}