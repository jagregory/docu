using Docu.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Docu
{
    public class ConsoleApplication
    {
        readonly DocumentationGenerator documentationGenerator;
        readonly IEventAggregator eventAggregator;

        public ConsoleApplication(DocumentationGenerator documentationGenerator, IEventAggregator eventAggregator)
        {
            this.documentationGenerator = documentationGenerator;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.GetEvent<WarningEvent>().Subscribe(message => Console.WriteLine("WARNING: " + message));
            this.eventAggregator.GetEvent<BadFileEvent>().Subscribe(path => Console.WriteLine("The requested file is in a bad format and could not be loaded: '" + path + "'"));
        }

        public void Run(List<string> args)
        {
            if (args.Count == 0 || args.Contains("--help"))
            {
                Console.WriteLine("usuage: docu pattern.dll [pattern.dll ...] [pattern.xml ...]");
                Console.WriteLine();
                Console.WriteLine(" * One or more dll matching patterns can be specified,");
                Console.WriteLine("   e.g. MyProject*.dll or MyProject.dll");
                Console.WriteLine();
                Console.WriteLine(" * Xml file names can be inferred from the dll patterns");
                Console.WriteLine("   or can be explicitly named.");
                Console.WriteLine();
                Console.WriteLine("Switches:");
                Console.WriteLine("  --help          Shows this message");
                Console.WriteLine("  --output=value  Sets the output path to value");
                return;
            }

            for (int i = args.Count - 1; i >= 0; i--)
            {
                if (args[i].StartsWith("--output="))
                {
                    documentationGenerator.SetOutputPath(args[i].Substring(9).TrimEnd('\\'));
                    args.RemoveAt(i);
                    continue;
                }
                if (args[i].StartsWith("--templates="))
                {
                    documentationGenerator.SetTemplatePath(args[i].Substring(12).TrimEnd('\\'));
                    args.RemoveAt(i);
                }
            }

            Console.WriteLine("-------------------------------");
            Console.WriteLine(" docu: simple docs done simply ");
            Console.WriteLine("-------------------------------");
            Console.WriteLine();

            string[] assemblies = GetAssembliesFromArgs(args);
            string[] xmls = GetXmlsFromArgs(args, assemblies);

            if (VerifyArguments(args, assemblies, xmls))
            {
                Console.WriteLine("Starting documentation generation");

                documentationGenerator.SetAssemblies(assemblies);
                documentationGenerator.SetXmlFiles(xmls);
                documentationGenerator.Generate();

                Console.WriteLine();
                Console.WriteLine("Generation complete");
            }
        }

        static string GetExpectedXmlFileForAssembly(string assembly)
        {
            string extension = Path.GetExtension(assembly);
            return assembly.Substring(0, assembly.Length - extension.Length) + ".xml";
        }

        static bool IsAssemblyArgument(string argument)
        {
            string fileExtension = Path.GetExtension(argument);
            return fileExtension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase)
                || fileExtension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase);
        }

        string[] GetAssembliesFromArgs(IEnumerable<string> args)
        {
            var assemblies = new List<string>();

            foreach (string arg in args)
            {
                if (IsAssemblyArgument(arg))
                {
                    assemblies.AddRange(GetFiles(arg));
                }
            }

            return assemblies.ToArray();
        }

        IEnumerable<string> GetFiles(string path)
        {
            if (path.Contains("*") || path.Contains("?"))
            {
                string dir = Environment.CurrentDirectory;
                string filename = Path.GetFileName(path);

                if (path.Contains("\\"))
                {
                    dir = Path.GetDirectoryName(path);
                }

                foreach (string file in Directory.GetFiles(dir, filename))
                {
                    yield return file.Replace(Environment.CurrentDirectory + "\\", string.Empty);
                }
            }
            else
            {
                yield return path;
            }
        }

        string[] GetXmlsFromArgs(IEnumerable<string> args, IEnumerable<string> assemblies)
        {
            var xmls = new List<string>();

            foreach (string arg in args)
            {
                if (arg.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    xmls.AddRange(GetFiles(arg));
                }
            }

            if (xmls.Count == 0)
            {
                // none specified, try to find some
                foreach (string assembly in assemblies)
                {
                    string name = GetExpectedXmlFileForAssembly(assembly);

                    foreach (string file in GetFiles(name))
                    {
                        if (File.Exists(file))
                        {
                            xmls.Add(file);
                        }
                    }
                }
            }

            return xmls.ToArray();
        }

        bool VerifyArguments(IEnumerable<string> arguments, IEnumerable<string> assemblies, IEnumerable<string> xmls)
        {
            foreach (string argument in arguments)
            {
                if (IsAssemblyArgument(argument)
                    || Path.GetExtension(argument).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Console.WriteLine("Invalid argument '" + argument + "': what am I supposed to do with this?");
                Console.WriteLine();
                Console.WriteLine("Use --help to see usage and switches");
                return false;
            }

            if (!VerifyAssemblies(assemblies))
            {
                return false;
            }

            if (!VerifyXmls(xmls))
            {
                return false;
            }

            return true;
        }

        bool VerifyAssemblies(IEnumerable<string> assemblies)
        {
            if (!assemblies.Any())
            {
                Console.WriteLine("No Assemblies specified: please give me some assemblies!");
                return false;
            }

            foreach (string assembly in assemblies)
            {
                if (!File.Exists(assembly))
                {
                    Console.WriteLine("Assembly not found '" + assembly + "': cannot continue.");
                    return false;
                }
            }

            return true;
        }

        bool VerifyXmls(IEnumerable<string> xmls)
        {
            if (!xmls.Any())
            {
                Console.WriteLine("No XML documents found: none were found for the assembly names you gave,");
                Console.WriteLine("explicitly specify them if their names differ from their associated assembly.");
                return false;
            }

            foreach (string xml in xmls)
            {
                if (!File.Exists(xml))
                {
                    Console.WriteLine("XML file not found '" + xml + "': cannot continue.");
                    return false;
                }
            }

            return true;
        }
    }
}