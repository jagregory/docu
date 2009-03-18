using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Docu.Documentation;

namespace Docu.Generation
{
    public class PatternTemplateResolver : IPatternTemplateResolver
    {
        private bool HACK_inNamespace;

        public IList<TemplateMatch> Resolve(string path, IList<AssemblyDoc> assemblies)
        {
            var current = path;

            if (current.Contains("\\"))
                current = current.Substring(0, current.IndexOf('\\'));

            return ResolveRecursive(current, path, path, assemblies, new OutputData { Assemblies = assemblies });
        }

        private IList<TemplateMatch> ResolveRecursive(string current, string outputPath, string templatePath, IEnumerable<AssemblyDoc> assemblies, OutputData data)
        {
            var matches = new List<TemplateMatch>();

            if (Path.GetExtension(current) == ".spark")
            {
                if (current == "!namespace.spark")
                {
                    foreach (var ns in from a in assemblies
                                       from n in a.Namespaces
                                       select n)
                    {
                        matches.Add(
                            new TemplateMatch(
                                outputPath.Replace(".spark", ".htm").Replace("!namespace", ns.Name),
                                templatePath,
                                new OutputData { Assemblies = data.Assemblies, Namespace = ns, Type = data.Type }
                                ));
                    }
                }
                else if (current == "!type.spark")
                {
                    foreach (var found in from a in assemblies
                                       from n in a.Namespaces
                                       from t in n.Types
                                       select new { Type = t, Namespace = n})
                    {
                        string name = HACK_inNamespace ? found.Type.Name : found.Namespace.Name + "." + found.Type.Name;

                        matches.Add(
                            new TemplateMatch(
                                outputPath.Replace(".spark", ".htm").Replace("!type", name),
                                templatePath,
                                new OutputData { Assemblies = data.Assemblies, Namespace = found.Namespace, Type = found.Type }
                                ));
                    }
                }
                else
                    matches.Add(
                        new TemplateMatch(
                            outputPath.Replace(".spark", ".htm"),
                            templatePath,
                            new OutputData
                            { Assemblies = data.Assemblies, Namespace = data.Namespace, Type = data.Type }
                            ));
            }
            else
            {
                if (!IsSpecial(current))
                {
                    string[] parts = templatePath.Replace(current, "").Split(new[] { '\\' },
                                                                             StringSplitOptions.RemoveEmptyEntries);

                    matches.AddRange(ResolveRecursive(parts[0], templatePath, templatePath, assemblies,
                                                      new OutputData
                                                      {
                                                          Assemblies = data.Assemblies,
                                                          Namespace = data.Namespace,
                                                          Type = data.Type
                                                      }));
                }
                else if (current == "!namespace")
                {
                    string[] parts =
                        templatePath.Substring(templatePath.IndexOf(current) + current.Length).Split(new[] { '\\' },
                                                                                                     StringSplitOptions.
                                                                                                         RemoveEmptyEntries);

                    HACK_inNamespace = false;

                    foreach (var found in from a in assemblies
                                       from n in a.Namespaces
                                       select new { Assembly = a, Namespace = n })
                    {
                        string nsPath = templatePath.Replace(current, found.Namespace.Name);

                        HACK_inNamespace = true;

                        var assembly = new AssemblyDoc(found.Assembly.Name);

                        assembly.Namespaces.Add(found.Namespace);

                        matches.AddRange(ResolveRecursive(parts[0], nsPath, templatePath, new List<AssemblyDoc> { assembly }, // bug
                                                          new OutputData
                                                          {
                                                              Assemblies = data.Assemblies,
                                                              Namespace = found.Namespace,
                                                              Type = data.Type
                                                          }));
                    }

                    HACK_inNamespace = false;
                }
                else if (current == "!type")
                {
                    string[] parts = templatePath.Replace(current, "").Split(new[] { '\\' },
                                                                             StringSplitOptions.RemoveEmptyEntries);

                    foreach (var found in from a in assemblies
                                          from n in a.Namespaces
                                          from t in n.Types
                                          select new { Type = t, Namespace = n })
                    {
                        string typePath = templatePath.Replace(current, found.Namespace.Name + "." + found.Type.Name);

                        matches.AddRange(ResolveRecursive(parts[0], typePath, templatePath,
                                                          new List<AssemblyDoc> {  }, // bug
                                                          new OutputData
                                                          {
                                                              Assemblies = data.Assemblies,
                                                              Namespace = found.Namespace,
                                                              Type = found.Type
                                                          }));
                    }
                }
            }

            return matches;
        }

        private bool IsSpecial(string name)
        {
            string path = Path.GetFileNameWithoutExtension(name);

            return path == "!namespace" || path == "!type" || name == "layouts";
        }
    }
}