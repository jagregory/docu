using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Generation;
using Spark;

namespace Docu.UI
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

        public IList<Assembly> Assemblies
        {
            get { return ViewData.Assemblies; }
        }

        public IList<Namespace> Namespaces
        {
            get { return ViewData.Namespaces; }
        }

        public IList<DeclaredType> Types
        {
            get { return ViewData.Types; }
        }

        public Namespace Namespace
        {
            get { return ViewData.Namespace; }
        }

        public DeclaredType Type
        {
            get { return ViewData.Type; }
        }

        public ViewData ViewData { get; set; }

        public string WriteProductName(Assembly assembly)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            
            return Formatter.Escape(info.ProductName);
        }

        public string WriteFileDescription(Assembly assembly)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);

            return Formatter.Escape(info.FileDescription);
        }

        public string WriteAssemblyTitle(Assembly assembly)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            string productName = info.ProductName;
            string fileDescription = info.FileDescription;
            if(String.IsNullOrEmpty(fileDescription) || (productName == fileDescription))
                return Formatter.Escape(productName);
            if(String.IsNullOrEmpty(productName))
                return Formatter.Escape(fileDescription);
            return Formatter.Escape(String.Format("{0} ({1})", fileDescription, productName));
        }

        public string WriteVersion(Assembly assembly)
        {
            return assembly.GetName().Version.ToString();
        }

        public string h(string content)
        {
            return Formatter.Escape(content);
        }

        public string Format(IComment comment)
        {
            return Formatter.Format(comment);
        }

        public string Format(IReferencable referencable)
        {
            return Formatter.FormatReferencable(referencable);
        }

        public string WriteInterfaces(IList<IReferencable> interfaces)
        {
            var sb = new StringBuilder();

            foreach (IReferencable face in interfaces)
            {
                sb.Append(Format(face));
                sb.Append(", ");
            }

            if (sb.Length > 0)
                sb.Length -= 2;

            return sb.ToString();
        }

        public string OutputMethodParams(Method method)
        {
            var sb = new StringBuilder();

            foreach (MethodParameter parameter in method.Parameters)
            {
                sb.Append(h(parameter.Reference.PrettyName));
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
    }
}