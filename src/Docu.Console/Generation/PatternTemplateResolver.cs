using System;
using System.Collections.Generic;
using System.IO;
using Docu.Documentation;

namespace Docu.Generation
{
    public class PatternTemplateResolver : IPatternTemplateResolver
    {
        private bool HACK_inNamespace;

        public IList<TemplateMatch> Resolve(string path, IList<Namespace> namespaces)
        {
            string current = path;

            if (current.Contains("\\"))
            {
                current = current.Substring(0, current.IndexOf('\\'));
            }

            return ResolveRecursive(current, path, path, namespaces, new OutputData { Namespaces = namespaces });
        }

        private IList<TemplateMatch> ResolveRecursive(string current, string outputPath, string templatePath,
                                                      IList<Namespace> namespaces, OutputData data)
        {
            var matches = new List<TemplateMatch>();

            if (Path.GetExtension(current) == ".spark")
            {
                if (current == "!namespace.spark")
                {
                    foreach (Namespace ns in namespaces)
                    {
                        matches.Add(
                            new TemplateMatch(
                                outputPath.Replace(".spark", ".htm").Replace("!namespace", ns.Name),
                                templatePath,
                                new OutputData { Namespaces = data.Namespaces, Namespace = ns, Type = data.Type }
                                ));
                    }
                }
                else if (current == "!type.spark")
                {
                    foreach (Namespace ns in namespaces)
                    {
                        foreach (DeclaredType type in ns.Types)
                        {
                            string name = HACK_inNamespace ? type.Name : ns.Name + "." + type.Name;

                            matches.Add(
                                new TemplateMatch(
                                    outputPath.Replace(".spark", ".htm").Replace("!type", name),
                                    templatePath,
                                    new OutputData { Namespaces = data.Namespaces, Namespace = ns, Type = type }
                                    ));
                        }
                    }
                }
                else
                    matches.Add(
                        new TemplateMatch(
                            outputPath.Replace(".spark", ".htm"),
                            templatePath,
                            new OutputData
                            { Namespaces = data.Namespaces, Namespace = data.Namespace, Type = data.Type }
                            ));
            }
            else
            {
                if (!IsSpecial(current))
                {
                    string[] parts = templatePath.Replace(current, "").Split(new[] { '\\' },
                                                                             StringSplitOptions.RemoveEmptyEntries);

                    matches.AddRange(ResolveRecursive(parts[0], templatePath, templatePath, namespaces,
                                                      new OutputData
                                                      {
                                                          Namespaces = data.Namespaces,
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

                    foreach (Namespace ns in namespaces)
                    {
                        string nsPath = templatePath.Replace(current, ns.Name);

                        HACK_inNamespace = true;

                        matches.AddRange(ResolveRecursive(parts[0], nsPath, templatePath, new List<Namespace> { ns },
                                                          new OutputData
                                                          {
                                                              Namespaces = data.Namespaces,
                                                              Namespace = ns,
                                                              Type = data.Type
                                                          }));
                    }

                    HACK_inNamespace = false;
                }
                else if (current == "!type")
                {
                    string[] parts = templatePath.Replace(current, "").Split(new[] { '\\' },
                                                                             StringSplitOptions.RemoveEmptyEntries);

                    foreach (Namespace ns in namespaces)
                    {
                        foreach (DeclaredType type in ns.Types)
                        {
                            string typePath = templatePath.Replace(current, ns.Name + "." + type.Name);

                            matches.AddRange(ResolveRecursive(parts[0], typePath, templatePath,
                                                              new List<Namespace> { ns },
                                                              new OutputData
                                                              {
                                                                  Namespaces = data.Namespaces,
                                                                  Namespace = ns,
                                                                  Type = type
                                                              }));
                        }
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