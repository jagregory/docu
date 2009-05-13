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
    /// All public or protected methods and properties on this class are available within documentation templates
    /// </summary>
    public abstract class SparkTemplateBase : AbstractSparkView
    {
        protected readonly IOutputFormatter Formatter = new HtmlOutputFormatter();

        /// <summary>
        /// All of the assemblies being documented
        /// </summary>
        public IList<Assembly> Assemblies
        {
            get { return ViewData.Assemblies; }
        }

        /// <summary>
        ///  All of the namespaces in the assemblies being documented
        /// </summary>
        public IList<Namespace> Namespaces
        {
            get { return ViewData.Namespaces; }
        }

        /// <summary>
        /// All of the types in the assemblies being documented
        /// </summary>
        public IList<DeclaredType> Types
        {
            get { return ViewData.Types; }
        }

        /// <summary>
        /// The current namespace being documented within the special !namespace template
        /// </summary>
        public Namespace Namespace
        {
            get { return ViewData.Namespace; }
        }

        /// <summary>
        /// The current type being documented withing the special !type template
        /// </summary>
        public DeclaredType Type
        {
            get { return ViewData.Type; }
        }

        public ViewData ViewData { get; set; }

        public void SetMethodUrlFormat(string format)
        {
            Formatter.MethodUrlFormat = format;
        }

        public void SetTypeUrlFormat(string format)
        {
            Formatter.TypeUrlFormat = format;
        }
        public void SetPropertyUrlFormat(string format)
        {
            Formatter.PropertyUrlFormat = format;
        }
        public void SetEventUrlFormat(string format)
        {
            Formatter.EventUrlFormat = format;
        }
        public void SetFieldUrlFormat(string format)
        {
            Formatter.FieldUrlFormat = format;
        }
        public void SetNamespaceUrlFormat(string format)
        {
            Formatter.NamespaceUrlFormat = format;
        }

        /// <summary>
        /// Returns the product name of an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public string WriteProductName(Assembly assembly)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            
            return Formatter.Escape(info.ProductName);
        }

        /// <summary>
        /// Returns the description of an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public string WriteFileDescription(Assembly assembly)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);

            return Formatter.Escape(info.FileDescription);
        }

        /// <summary>
        /// Returns the title of an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the version number of an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public string WriteVersion(Assembly assembly)
        {
            return assembly.GetName().Version.ToString();
        }

        /// <summary>
        /// HTML encodes the content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string h(string content)
        {
            return Formatter.Escape(content);
        }

        /// <summary>
        /// Returns the comments in a format suitable for display
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public string Format(IComment comment)
        {
            return Formatter.Format(comment);
        }

        /// <summary>
        /// Returns a hyperlink to another symbol
        /// </summary>
        /// <remarks>The format of the URL in the returned hyperlink can be controlled by the methods <see cref="SetNamespaceUrlFormat"/>, <see cref="SetTypeUrlFormat"/>,  <see cref="SetPropertyUrlFormat"/>, <see cref="SetMethodUrlFormat"/>, <see cref="SetFieldUrlFormat"/> and <see cref="SetEventUrlFormat"/></remarks>
        /// <param name="referencable"></param>
        /// <returns></returns>
        public string Format(IReferencable referencable)
        {
            return Formatter.FormatReferencable(referencable);
        }

        /// <summary>
        /// Returns a comma-delimited list of the interfaces impleted by a given type
        /// </summary>
        /// <param name="interfaces"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a comma-delimited list of the parameters of a given method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public string OutputMethodParams(Method method)
        {
            var sb = new StringBuilder();
            var markExtensionMethodInstance = method.IsExtension;

            foreach (MethodParameter parameter in method.Parameters)
            {
                if (markExtensionMethodInstance)
                {
                    sb.Append("this ");
                    markExtensionMethodInstance = false;
                }
                sb.Append(h(parameter.Reference.PrettyName));
                sb.Append(" ");
                sb.Append(parameter.Name);
                sb.Append(", ");
            }

            if (sb.Length > 0)
                sb.Length -= 2;

            return sb.ToString();
        }
    }
}