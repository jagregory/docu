using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Spark;

namespace Docu.Output.Rendering
{
    public interface IDocuTemplate
    {
        string SiteResource(string path);
    }

    /// <summary>
    /// All public or protected methods and properties on this class are available within documentation templates
    /// </summary>
    public abstract class SparkTemplateBase : AbstractSparkView, IDocuTemplate
    {
        protected readonly HtmlOutputFormatter Formatter;

        protected SparkTemplateBase()
        {
            Formatter = new HtmlOutputFormatter(this);
        }

        public string SiteResource(string path)
        {
            if (!path.StartsWith("~/")) return path;

            var formattedPath = path.Replace("~/", "");
            var outputPathDepth = (RelativeOutputPath ?? "").GetDirectoryDepth();

            return outputPathDepth.Times("../") + formattedPath;
        }

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

        /// <summary>
        /// The path of the file that this view represents, relative to the output directory.
        /// </summary>
        public string RelativeOutputPath { get; set; }

        /// <summary>
        /// Configures the pattern that will be used to construct links to methods referenced in the documentation of other symbols
        /// </summary>
        /// <remarks>
        /// The pattern can be constructed using the following placeholders:
        /// <list type="definition">
        /// <item><term>{type.namespace}</term><description>The namespace of the type that contains the method</description></item>
        /// <item><term>{type}</term><description>The short name of the type that contains the method</description></item>
        /// <item><term>{method}</term><description>The name of method</description></item>
        /// </list>
        /// <para>The default is {type.namespace}/{type}.htm#{method}</para></remarks>
        /// <param name="format">The pattern used to construct the link</param>
        public void SetMethodUrlFormat(string format)
        {
            Formatter.MethodUrlFormat = format;
        }

        /// <summary>
        /// Configures the pattern that will be used to construct links to types referenced in the documentation of other symbols
        /// </summary>
        /// <remarks>
        /// The pattern can be constructed using the following placeholders:
        /// <list type="definition">
        /// <item><term>{type.namespace}</term><description>The namespace of the type</description></item>
        /// <item><term>{type}</term><description>The short name of the type</description></item>
        /// </list>
        /// <para>The default is {type.namespace}/{type}.htm</para></remarks>
        /// <param name="format">The pattern used to construct the link</param>
        public void SetTypeUrlFormat(string format)
        {
            Formatter.TypeUrlFormat = format;
        }
        
        /// <summary>
        /// Configures the pattern that will be used to construct links to properties referenced in the documentation of other symbols
        /// </summary>
        /// <remarks>
        /// The pattern can be constructed using the following placeholders:
        /// <list type="definition">
        /// <item><term>{type.namespace}</term><description>The namespace of the type that contains the property</description></item>
        /// <item><term>{type}</term><description>The short name of the type that contains the property</description></item>
        /// <item><term>{property}</term><description>The name of the property</description></item>
        /// </list>
        /// <para>The default is {type.namespace}/{type}.htm#{property}</para></remarks>
        /// <param name="format">The pattern used to construct the link</param>
        public void SetPropertyUrlFormat(string format)
        {
            Formatter.PropertyUrlFormat = format;
        }

        /// <summary>
        /// Configures the pattern that will be used to construct links to events referenced in the documentation of other symbols
        /// </summary>
        /// <remarks>
        /// The pattern can be constructed using the following placeholders:
        /// <list type="definition">
        /// <item><term>{type.namespace}</term><description>The namespace of the type that contains the event</description></item>
        /// <item><term>{type}</term><description>The short name of the type that contains the event</description></item>
        /// <item><term>{event}</term><description>The name of the event</description></item>
        /// </list>
        /// <para>The default is {type.namespace}/{type}.htm#{event}</para></remarks>
        /// <param name="format">The pattern used to construct the link</param>
        public void SetEventUrlFormat(string format)
        {
            Formatter.EventUrlFormat = format;
        }

        /// <summary>
        /// Configures the pattern that will be used to construct links to fields referenced in the documentation of other symbols
        /// </summary>
        /// <remarks>
        /// The pattern can be constructed using the following placeholders:
        /// <list type="definition">
        /// <item><term>{type.namespace}</term><description>The namespace of the type that contains the field</description></item>
        /// <item><term>{type}</term><description>The short name of the type that contains the field</description></item>
        /// <item><term>{field}</term><description>The name of the field</description></item>
        /// </list>
        /// <para>The default is {type.namespace}/{type}.htm#{field}</para></remarks>
        /// <param name="format">The pattern used to construct the link</param>
        public void SetFieldUrlFormat(string format)
        {
            Formatter.FieldUrlFormat = format;
        }

        /// <summary>
        /// Configures the pattern that will be used to construct links to namespaces referenced in the documentation of other symbols
        /// </summary>
        /// <remarks>
        /// The pattern can be constructed using the following placeholders:
        /// <list type="definition">
        /// <item><term>{type.namespace}</term><description>The name of the namespace</description></item>
        /// </list>
        /// <para>The default is {namespace}.htm</para></remarks>
        /// <param name="format">The pattern used to construct the link</param>
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
        public string Format(Comment comment)
        {
            return Formatter.Format(comment);
        }

        /// <summary>
        /// Returns a hyperlink to another symbol
        /// </summary>
        /// <remarks>The format of the URL in the returned hyperlink can be controlled by the methods <see cref="SetNamespaceUrlFormat"/>, <see cref="SetTypeUrlFormat"/>,  <see cref="SetPropertyUrlFormat"/>, <see cref="SetMethodUrlFormat"/>, <see cref="SetFieldUrlFormat"/> and <see cref="SetEventUrlFormat"/></remarks>
        /// <returns></returns>
        public string Format(IReferencable referencable, params Expression<Func<object, string>>[] attributes)
        {
            var attributeDictionary = new Dictionary<string, string>();

            attributes.ForEach(exp =>
            {
                var name = exp.Parameters.First().Name;
                var value = ((ConstantExpression)exp.Body).Value.ToString();
                
                attributeDictionary.Add(name, value);
            });

            return Formatter.FormatReferencable(referencable, attributeDictionary);
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
                sb.Append(Format(parameter.Reference));
                sb.Append(" ");
                sb.Append(parameter.Name);
                sb.Append(", ");
            }

            if (sb.Length > 0)
                sb.Length -= 2;

            return sb.ToString();
        }
    }

    public static class StringPathExtensions
    {
        public static bool IsRoot(this string path)
        {
            return !path.Contains("\\");
        }

        public static int GetDirectoryDepth(this string path)
        {
            return path.Count(x => x == '\\');
        }

        public static string Times(this int times, string value)
        {
            var result = "";

            for (var i = 0; i < times; i++)
                result += value;

            return result;
        }
    }
}