using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Docu.Documentation;

namespace Docu.Generation
{
    public class PatternTemplateResolver : IPatternTemplateResolver
    {
        private bool HACK_inNamespace;
        private readonly List<TemplateMatch> matches = new List<TemplateMatch>();
        private IEnumerable<Assembly> usedAssemblies = new Assembly[0];

        public void SetAssemblies(IEnumerable<Assembly> assemblies)
        {
            usedAssemblies = assemblies;
        }

        /// <summary>
        /// Expands a path into a collection of output files
        /// </summary>
        /// <example>
        /// given template path and a list of namespaces:
        /// 
        /// test.htm.spark, null                  == [test.htm]
        /// !namespace.htm.spark, [one, two]      == [one.htm, two.htm]
        /// !namespace/test.htm.spark, [one, two] == [one/test.htm, two/test.htm]
        /// </example>
        /// <param name="path">Template path</param>
        /// <param name="namespaces">Namespaces</param>
        /// <returns>List of resolved output matches</returns>
        public IList<TemplateMatch> Resolve(string path, IList<Namespace> namespaces)
        {
            var parts = new List<string>(path.Split('\\'));
            var head = parts[0];

            return ResolveRecursive(head, parts.ToTail(), path, path, namespaces, new ViewData { Namespaces = namespaces, Assemblies = new List<Assembly>(usedAssemblies) });
        }

        private IList<TemplateMatch> ResolveRecursive(string head, IList<string> tail, string outputPath, string templatePath, IEnumerable<Namespace> namespaces, ViewData data)
        {
            if (Path.GetExtension(head) == ".spark")
                ResolveTemplate(head, outputPath, templatePath, namespaces, data);
            else
                ResolveDirectory(head, templatePath, namespaces, data, tail);

            return matches;
        }

        private void ResolveTemplate(string head, string outputPath, string templatePath, IEnumerable<Namespace> namespaces, ViewData data)
        {
            if (head.StartsWith("!namespace"))
                ResolveNamespaceTemplate(outputPath, templatePath, namespaces, data);
            else if (head.StartsWith("!type"))
                ResolveTypeTemplate(outputPath, templatePath, namespaces, data);
            else
                AddMatch(outputPath, templatePath, data);
        }

        private void ResolveNamespaceTemplate(string outputPath, string templatePath, IEnumerable<Namespace> namespaces, ViewData data)
        {
            foreach (var ns in namespaces)
            {
                AddMatch(outputPath.Replace("!namespace", ns.Name), templatePath, data, ns);
            }
        }

        private void ResolveTypeTemplate(string outputPath, string templatePath, IEnumerable<Namespace> namespaces, ViewData data)
        {
            var foundTypes = from n in namespaces
                             from t in n.Types
                             select new { Type = t, Namespace = n };
            foreach (var found in foundTypes)
            {
                string name = HACK_inNamespace ? found.Type.Name : found.Namespace.Name + "." + found.Type.Name;

                AddMatch(outputPath.Replace("!type", name), templatePath, data, found.Namespace, found.Type);
            }
        }

        private void ResolveDirectory(string head, string templatePath, IEnumerable<Namespace> namespaces, ViewData data, IList<string> tail)
        {
            if (!IsSpecial(head))
                ResolveRecursive(tail[0], tail.ToTail(), templatePath, templatePath, namespaces, data.Clone());
            else if (head == "!namespace")
                ResolveNamespaceDirectory(head, tail, templatePath, namespaces, data);
            else if (head == "!type")
                ResolveTypeDirectory(head, tail, templatePath, namespaces, data);
        }

        private void ResolveNamespaceDirectory(string head, IList<string> tail, string templatePath, IEnumerable<Namespace> namespaces, ViewData data)
        {
            HACK_inNamespace = false;

            foreach (var ns in namespaces)
            {
                string nsPath = templatePath.Replace(head, ns.Name);

                HACK_inNamespace = true;

                var clone = data.Clone();

                clone.Namespace = ns;

                ResolveRecursive(tail[0], tail.ToTail(), nsPath, templatePath, new List<Namespace> { ns }, clone);
            }

            HACK_inNamespace = false;
        }

        private void ResolveTypeDirectory(string head, IList<string> tail, string templatePath, IEnumerable<Namespace> namespaces, ViewData data)
        {
            foreach (var found in from n in namespaces
                                  from t in n.Types
                                  select new { Type = t, Namespace = n })
            {
                string typePath = templatePath.Replace(head, found.Namespace.Name + "." + found.Type.Name);

                var clone = data.Clone();

                clone.Namespace = found.Namespace;
                clone.Type = found.Type;

                ResolveRecursive(tail[0], tail.ToTail(), typePath, templatePath, new List<Namespace> { found.Namespace }, clone);
            }
        }

        private void AddMatch(string outputPath, string templatePath, ViewData data)
        {
            var path = outputPath.Replace(".spark", "");
            var clone = data.Clone();

            if (clone.Types.Count == 0)
                clone.Types = (from n in data.Namespaces from t in n.Types select t).ToList();

            matches.Add(new TemplateMatch(path, templatePath, clone));
        }

        private void AddMatch(string outputPath, string templatePath, ViewData data, Namespace ns)
        {
            var clone = data.Clone();

            clone.Namespace = ns;

            AddMatch(outputPath, templatePath, clone);
        }

        private void AddMatch(string outputPath, string templatePath, ViewData data, Namespace ns, DeclaredType type)
        {
            var clone = data.Clone();

            clone.Type = type;

            AddMatch(outputPath, templatePath, clone, ns);
        }

        private bool IsSpecial(string name)
        {
            string path = Path.GetFileNameWithoutExtension(name);

            return path == "!namespace" || path == "!type" || name == "layouts";
        }
    }
}
