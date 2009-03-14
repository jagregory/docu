using System;
using System.Collections.Generic;
using System.Text;
using Spark;

namespace DrDoc.Generation
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
        public string flatten(IList<DocBlock> blocks)
        {
            var sb = new StringBuilder();

            foreach (var block in blocks)
            {
                sb.Append(block.ToString());
            }

            return sb.ToString();
        }

        public string h(string content)
        {
            return content.Replace("<", "&lt;").Replace(">", "&gt;");
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

        public IList<DocNamespace> Namespaces
        {
            get { return ViewData.Namespaces; }
        }

        public DocNamespace Namespace
        {
            get { return ViewData.Namespace; }
        }

        public DocType Type
        {
            get { return ViewData.Type; }
        }

        public OutputData ViewData { get; set; }
    }
}