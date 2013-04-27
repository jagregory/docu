namespace Docu.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Docu.Events;

    using StructureMap;

    public class ConsoleApplication
    {
        private readonly List<string> arguments = new List<string>();

        private readonly IDocumentationGenerator documentationGenerator;

        private readonly IEventAggregator eventAggregator;

        private readonly IScreenWriter screenWriter;

        private readonly IList<ISwitch> switches = new List<ISwitch>();

        private bool canRun;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleApplication"/> class.
        /// </summary>
        /// <param name="screenWriter">
        /// The screen writer.
        /// </param>
        /// <param name="documentationGenerator">
        /// The documentation generator.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        public ConsoleApplication(
            IScreenWriter screenWriter, IDocumentationGenerator documentationGenerator, IEventAggregator eventAggregator)
        {
            this.screenWriter = screenWriter;
            this.documentationGenerator = documentationGenerator;
            this.eventAggregator = eventAggregator;

            this.WireUpListeners();
            this.DefineSwitches();
        }

        public static void Run(IEnumerable<string> args)
        {
            IContainer container = ContainerBootstrapper.BootstrapStructureMap();

            var application = container.GetInstance<ConsoleApplication>();

            application.SetArguments(args);
            application.Run();
        }

        public void Run()
        {
            if (!this.canRun)
            {
                this.ShowMessage(Messages.Help);
                return;
            }

            if (this.ProcessSwitches() == false)
            {
                return;
            }

            this.ShowMessage(Messages.Splash);

            string[] assemblies = this.GetAssembliesFromArgs(this.arguments);
            string[] xmls = this.GetXmlsFromArgs(this.arguments, assemblies);

            if (this.VerifyArguments(assemblies, xmls))
            {
                this.ShowMessage(Messages.Start);

                this.documentationGenerator.SetAssemblies(assemblies);
                this.documentationGenerator.SetXmlFiles(xmls);
                this.documentationGenerator.Generate();

                this.ShowMessage(Messages.Done);
            }
        }

        public void SetArguments(IEnumerable<string> args)
        {
            this.arguments.AddRange(args);

            if (this.arguments.Count > 0)
            {
                this.canRun = true;
            }
        }

        private static string getExpectedXmlFileForAssembly(string assembly)
        {
            string extension = Path.GetExtension(assembly);
            return assembly.Substring(0, assembly.Length - extension.Length) + ".xml";
        }

        private static bool isAssemblyArgument(string argument)
        {
            string fileExtension = Path.GetExtension(argument);
            return fileExtension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase)
                   || fileExtension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase);
        }

        private void BadFile(string path)
        {
            this.ShowMessage(new BadFileMessage(path));
        }

        private void DefineSwitches()
        {
            this.switches.Add(
                new Switch(
                    "--help", 
                    () =>
                        {
                            this.ShowMessage(new HelpMessage());
                            return false;
                        }));

            this.switches.Add(
                new ParameterSwitch(
                    "--output", 
                    arg =>
                        {
                            this.documentationGenerator.SetOutputPath(arg.TrimEnd('\\'));
                            return true;
                        }));

            this.switches.Add(
                new ParameterSwitch(
                    "--templates", 
                    arg =>
                        {
                            this.documentationGenerator.SetTemplatePath(arg.TrimEnd('\\'));
                            return true;
                        }));
        }

        private string[] GetAssembliesFromArgs(IEnumerable<string> args)
        {
            var assemblies = new List<string>();

            foreach (string arg in args)
            {
                if (isAssemblyArgument(arg))
                {
                    assemblies.AddRange(this.GetFiles(arg));
                }
            }

            return assemblies.ToArray();
        }

        private IEnumerable<string> GetFiles(string path)
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

        private string[] GetXmlsFromArgs(IEnumerable<string> args, IEnumerable<string> assemblies)
        {
            var xmls = new List<string>();

            foreach (string arg in args)
            {
                if (arg.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    xmls.AddRange(this.GetFiles(arg));
                }
            }

            if (xmls.Count == 0)
            {
                // none specified, try to find some
                foreach (string assembly in assemblies)
                {
                    string name = getExpectedXmlFileForAssembly(assembly);

                    foreach (string file in this.GetFiles(name))
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

        private bool ProcessSwitches()
        {
            foreach (ISwitch svvitch in this.switches)
            {
                for (int i = this.arguments.Count - 1; i >= 0; i--)
                {
                    string argument = this.arguments[i];

                    if (!svvitch.IsMatch(argument))
                    {
                        continue;
                    }

                    this.arguments.RemoveAt(i);

                    if (svvitch.Handle(argument) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ShowMessage(IScreenMessage message)
        {
            this.screenWriter.WriteMessage(message);
        }

        private bool VerifyArguments(IEnumerable<string> assemblies, IEnumerable<string> xmls)
        {
            foreach (string argument in this.arguments)
            {
                if (isAssemblyArgument(argument)
                    || Path.GetExtension(argument).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                this.ShowMessage(new InvalidArgumentMessage(argument));
                return false;
            }

            if (!this.VerifyAssemblies(assemblies))
            {
                return false;
            }

            if (!this.VerifyXmls(xmls))
            {
                return false;
            }

            return true;
        }

        private bool VerifyAssemblies(IEnumerable<string> assemblies)
        {
            if (!assemblies.Any())
            {
                this.ShowMessage(Messages.NoAssembliesSpecified);
                return false;
            }

            foreach (string assembly in assemblies)
            {
                if (!File.Exists(assembly))
                {
                    this.ShowMessage(new AssemblyNotFoundMessage(assembly));
                    return false;
                }
            }

            return true;
        }

        private bool VerifyXmls(IEnumerable<string> xmls)
        {
            if (!xmls.Any())
            {
                this.ShowMessage(Messages.NoXmlsFound);
                return false;
            }

            foreach (string xml in xmls)
            {
                if (!File.Exists(xml))
                {
                    this.ShowMessage(new XmlNotFoundMessage(xml));
                    return false;
                }
            }

            return true;
        }

        private void Warning(string message)
        {
            this.ShowMessage(new WarningMessage(message));
        }

        private void WireUpListeners()
        {
            this.eventAggregator.GetEvent<WarningEvent>().Subscribe(this.Warning);
            this.eventAggregator.GetEvent<BadFileEvent>().Subscribe(this.BadFile);
        }
    }
}