using Docu.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Docu.Console
{
    public class ConsoleApplication
    {
        readonly List<string> arguments = new List<string>();
        readonly DocumentationGenerator documentationGenerator;
        readonly IEventAggregator eventAggregator;
        readonly IList<ISwitch> switches = new List<ISwitch>();

        public ConsoleApplication(DocumentationGenerator documentationGenerator, IEventAggregator eventAggregator)
        {
            this.documentationGenerator = documentationGenerator;
            this.eventAggregator = eventAggregator;

            WireUpListeners();
            DefineSwitches();
        }

        public void Run(IEnumerable<string> args)
        {
            arguments.AddRange(args);

            if (arguments.Count == 0)
            {
                ShowHelp();
                return;
            }

            if (ProcessSwitches() == false)
            {
                return;
            }

            System.Console.WriteLine("-------------------------------");
            System.Console.WriteLine(" docu: simple docs done simply ");
            System.Console.WriteLine("-------------------------------");
            System.Console.WriteLine();

            string[] assemblies = GetAssembliesFromArgs(arguments);
            string[] xmls = GetXmlsFromArgs(arguments, assemblies);

            if (VerifyArguments(assemblies, xmls))
            {
                System.Console.WriteLine("Starting documentation generation");

                documentationGenerator.SetAssemblies(assemblies);
                documentationGenerator.SetXmlFiles(xmls);
                documentationGenerator.Generate();

                System.Console.WriteLine();
                System.Console.WriteLine("Generation complete");
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

        void DefineSwitches()
        {
            switches.Add(
                new Switch(
                    "--help",
                    () =>
                        {
                            ShowHelp();
                            return false;
                        }));

            switches.Add(
                new ParameterSwitch(
                    "--output",
                    arg =>
                        {
                            documentationGenerator.SetOutputPath(arg.TrimEnd('\\'));
                            return true;
                        }));

            switches.Add(
                new ParameterSwitch(
                    "--templates",
                    arg =>
                        {
                            documentationGenerator.SetTemplatePath(arg.TrimEnd('\\'));
                            return true;
                        }));
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

        bool ProcessSwitches()
        {
            foreach (ISwitch svvitch in switches)
            {
                for (int i = arguments.Count - 1; i >= 0; i--)
                {
                    string argument = arguments[i];

                    if (!svvitch.IsMatch(argument))
                    {
                        continue;
                    }

                    arguments.RemoveAt(i);

                    if (svvitch.Handle(argument) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        bool VerifyArguments(IEnumerable<string> assemblies, IEnumerable<string> xmls)
        {
            foreach (string argument in arguments)
            {
                if (IsAssemblyArgument(argument)
                    || Path.GetExtension(argument).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                System.Console.WriteLine("Invalid argument '" + argument + "': what am I supposed to do with this?");
                System.Console.WriteLine();
                System.Console.WriteLine("Use --help to see usage and switches");
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
                System.Console.WriteLine("No Assemblies specified: please give me some assemblies!");
                return false;
            }

            foreach (string assembly in assemblies)
            {
                if (!File.Exists(assembly))
                {
                    System.Console.WriteLine("Assembly not found '" + assembly + "': cannot continue.");
                    return false;
                }
            }

            return true;
        }

        bool VerifyXmls(IEnumerable<string> xmls)
        {
            if (!xmls.Any())
            {
                System.Console.WriteLine("No XML documents found: none were found for the assembly names you gave,");
                System.Console.WriteLine("explicitly specify them if their names differ from their associated assembly.");
                return false;
            }

            foreach (string xml in xmls)
            {
                if (!File.Exists(xml))
                {
                    System.Console.WriteLine("XML file not found '" + xml + "': cannot continue.");
                    return false;
                }
            }

            return true;
        }

        void ShowHelp()
        {
            System.Console.WriteLine("usuage: docu pattern.dll [pattern.dll ...] [pattern.xml ...]");
            System.Console.WriteLine();
            System.Console.WriteLine(" * One or more dll matching patterns can be specified,");
            System.Console.WriteLine("   e.g. MyProject*.dll or MyProject.dll");
            System.Console.WriteLine();
            System.Console.WriteLine(" * Xml file names can be inferred from the dll patterns");
            System.Console.WriteLine("   or can be explicitly named.");
            System.Console.WriteLine();
            System.Console.WriteLine("Switches:");
            System.Console.WriteLine("  --help          Shows this message");
            System.Console.WriteLine("  --output=value  Sets the output path to value");
        }

        void WireUpListeners()
        {
            eventAggregator.GetEvent<WarningEvent>().Subscribe(message => System.Console.WriteLine("WARNING: " + message));
            eventAggregator.GetEvent<BadFileEvent>().Subscribe(path => System.Console.WriteLine("The requested file is in a bad format and could not be loaded: '" + path + "'"));
        }
    }
}