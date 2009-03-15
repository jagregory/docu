using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DrDoc.Documentation;
using DrDoc.Generation;
using DrDoc.IO;
using DrDoc.Parsing;
using StructureMap;

namespace DrDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            ContainerBootstrapper.BootstrapStructureMap();

            if (args.Length == 0)
            {
                Console.WriteLine("Nothing to do!");
                return;
            }

            var assemblies = GetAssembliesFromArgs(args);
            var xmls = GetXmlsFromArgs(args, assemblies);
            var loadedAssemblies = LoadAssemblies(assemblies);

            var documentationGenerator = ObjectFactory.GetInstance<DocumentationGenerator>();

            documentationGenerator.SetAssemblies(loadedAssemblies);
            documentationGenerator.SetXmlContent(xmls);
            documentationGenerator.Generate();

            Console.WriteLine("Done");
        }

        private static string[] GetXmlsFromArgs(string[] args, string[] assemblies)
        {
            var xmls = new List<string>();

            foreach (var arg in args)
            {
                if (arg.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                    xmls.Add(File.ReadAllText(arg));
            }

            if (xmls.Count == 0)
            {
                // none specified, try to find some
                foreach (var assembly in assemblies)
                {
                    var name = assembly.Replace(".dll", ".xml");

                    if (File.Exists(name))
                        xmls.Add(File.ReadAllText(name));
                }
            }

            return xmls.ToArray();
        }

        private static Assembly[] LoadAssemblies(string[] assemblyNames)
        {
            var assemblies = new List<Assembly>();

            foreach (var name in assemblyNames)
            {
                assemblies.Add(Assembly.LoadFrom(name));
            }

            return assemblies.ToArray();
        }

        private static string[] GetAssembliesFromArgs(string[] args)
        {
            var assemblies = new List<string>();

            foreach (var arg in args)
            {
                if (!arg.StartsWith("/"))
                    assemblies.Add(arg);
            }

            return assemblies.ToArray();
        }
    }
}
