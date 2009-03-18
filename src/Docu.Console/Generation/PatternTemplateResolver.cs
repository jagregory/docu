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
        private IEnumerable<Assembly> usedAssemblies;

        public IList<TemplateMatch> Resolve(string path, IList<Namespace> namespaces)
        {
            var current = path;

            if (current.Contains("\\"))
                current = current.Substring(0, current.IndexOf('\\'));

            return ResolveRecursive(current, path, path, namespaces, new ViewData { Namespaces = namespaces, Assemblies = new List<Assembly>(usedAssemblies) });
        }

        public void SetAssemblies(IEnumerable<Assembly> assemblies)
        {
            usedAssemblies = assemblies;
        }

        private IList<TemplateMatch> ResolveRecursive(string current, string outputPath, string templatePath, IEnumerable<Namespace> namespaces, ViewData data)
        {
            if (Path.GetExtension(current) == ".spark")
            {
                if (current == "!namespace.spark")
                {
                    foreach (var ns in namespaces)
                    {
                        AddMatch(outputPath.Replace("!namespace", ns.Name), templatePath, data, ns);
                    }
                }
                else if (current == "!type.spark")
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
                else
                    AddMatch(outputPath, templatePath, data);
            }
            else
            {
                if (!IsSpecial(current))
                {
                    string[] parts = templatePath.Substring(templatePath.IndexOf(current) + current.Length).Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                    ResolveRecursive(parts[0], templatePath, templatePath, namespaces, data.Clone());
                }
                else if (current == "!namespace")
                {
                    string[] parts = templatePath.Substring(templatePath.IndexOf(current) + current.Length).Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                    HACK_inNamespace = false;

                    foreach (var ns in namespaces)
                    {
                        string nsPath = templatePath.Replace(current, ns.Name);

                        HACK_inNamespace = true;

                        var clone = data.Clone();

                        clone.Namespace = ns;

                        ResolveRecursive(parts[0], nsPath, templatePath, new List<Namespace> { ns }, clone);
                    }

                    HACK_inNamespace = false;
                }
                else if (current == "!type")
                {
                    string[] parts = templatePath.Replace(current, "").Split(new[] { '\\' },
                                                                             StringSplitOptions.RemoveEmptyEntries);

                    foreach (var found in from n in namespaces
                                          from t in n.Types
                                          select new { Type = t, Namespace = n })
                    {
                        string typePath = templatePath.Replace(current, found.Namespace.Name + "." + found.Type.Name);

                        var clone = data.Clone();

                        clone.Namespace = found.Namespace;
                        clone.Type = found.Type;

                        ResolveRecursive(parts[0], typePath, templatePath, new List<Namespace> { found.Namespace }, clone);
                    }
                }
            }

            return matches;
        }

        private void AddMatch(string outputPath, string templatePath, ViewData data)
        {
            var path = outputPath.Replace(".spark", ".htm");
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