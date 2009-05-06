using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Docu.Events;
using StructureMap;

namespace Docu.Console
{
    public class ConsoleApplication
    {
        private readonly List<string> arguments = new List<string>();
        private readonly IDocumentationGenerator documentationGenerator;
        private readonly IEventAggregator eventAggregator;
        private readonly IScreenWriter screenWriter;
        private bool canRun;
        private readonly IList<ISwitch> switches = new List<ISwitch>();

        public ConsoleApplication(IScreenWriter screenWriter, IDocumentationGenerator documentationGenerator, IEventAggregator eventAggregator)
        {
            this.screenWriter = screenWriter;
            this.documentationGenerator = documentationGenerator;
            this.eventAggregator = eventAggregator;

            WireUpListeners();
            DefineSwitches();
        }

        private void WireUpListeners()
        {
            eventAggregator
                .GetEvent<WarningEvent>()
                .Subscribe(Warning);
            eventAggregator
                .GetEvent<BadFileEvent>()
                .Subscribe(BadFile);
        }

        private void DefineSwitches()
        {
            switches.Add(new Switch("--help", () =>
            {
                ShowMessage(new HelpMessage());
                return false;
            }));
            switches.Add(new ParameterSwitch("--output", arg =>
            {
                documentationGenerator.SetOutputPath(arg);
                return true;
            }));
            switches.Add(new ParameterSwitch("--templates", arg =>
            {
                documentationGenerator.SetTemplatePath(arg);
                return true;
            }));
        }

        void Warning(string message)
        {
            ShowMessage(new WarningMessage(message));
        }

        void BadFile(string path)
        {
            ShowMessage(new BadFileMessage(path));
        }

        public void SetArguments(IEnumerable<string> args)
        {
            arguments.AddRange(args);

            if (arguments.Count > 0)
                canRun = true;
        }

        public void Run()
        {
            if (!canRun)
            {
                ShowMessage(Messages.Help);
                return;
            }

            if (ProcessSwitches() == false)
                return;

            ShowMessage(Messages.Splash);

            string[] assemblies = GetAssembliesFromArgs(arguments);
            string[] xmls = GetXmlsFromArgs(arguments, assemblies);

            if (VerifyArguments(assemblies, xmls))
            {
                ShowMessage(Messages.Start);

                documentationGenerator.SetAssemblies(assemblies);
                documentationGenerator.SetXmlFiles(xmls);
                documentationGenerator.Generate();

                ShowMessage(Messages.Done);
            }
        }

        private bool ProcessSwitches()
        {
            foreach (var svvitch in switches)
            {
                for (var i = arguments.Count - 1; i >= 0; i--)
                {
                    var argument = arguments[i];

                    if (!svvitch.IsMatch(argument)) continue;

                    arguments.RemoveAt(i);

                    if (svvitch.Handle(argument) == false)
                        return false;
                }
            }

            return true;
        }

        private bool VerifyArguments(IEnumerable<string> assemblies, IEnumerable<string> xmls)
        {
            foreach (string argument in arguments)
            {
                if (argument.EndsWith(".dll") || argument.EndsWith(".xml")) continue;

                ShowMessage(new InvalidArgumentMessage(argument));
                return false;
            }

            if (!VerifyAssemblies(assemblies)) return false;
            if (!VerifyXmls(xmls)) return false;

            return true;
        }

        private bool VerifyAssemblies(IEnumerable<string> assemblies)
        {
            if (assemblies.Count() == 0)
            {
                ShowMessage(Messages.NoAssembliesSpecified);
                return false;
            }

            foreach (string assembly in assemblies)
            {
                if (!File.Exists(assembly))
                {
                    ShowMessage(new AssemblyNotFoundMessage(assembly));
                    return false;
                }
            }

            return true;
        }

        private bool VerifyXmls(IEnumerable<string> xmls)
        {
            if (xmls.Count() == 0)
            {
                ShowMessage(Messages.NoXmlsFound);
                return false;
            }

            foreach (string xml in xmls)
            {
                if (!File.Exists(xml))
                {
                    ShowMessage(new XmlNotFoundMessage(xml));
                    return false;
                }
            }

            return true;
        }

        private void ShowMessage(IScreenMessage message)
        {
            screenWriter.WriteMessage(message);
        }

        private string[] GetXmlsFromArgs(IEnumerable<string> args, IEnumerable<string> assemblies)
        {
            var xmls = new List<string>();

            foreach (var arg in args)
            {
                if (arg.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                    xmls.AddRange(GetFiles(arg));
            }

            if (xmls.Count == 0)
            {
                // none specified, try to find some
                foreach (string assembly in assemblies)
                {
                    var name = assembly.Replace(".dll", ".xml");

                    foreach (var file in GetFiles(name))
                    {
                        if (File.Exists(file))
                            xmls.Add(file);
                    }
                }
            }

            return xmls.ToArray();
        }

        private string[] GetAssembliesFromArgs(IEnumerable<string> args)
        {
            var assemblies = new List<string>();

            foreach (var arg in args)
            {
                if (arg.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                    assemblies.AddRange(GetFiles(arg));
            }

            return assemblies.ToArray();
        }

        private IEnumerable<string> GetFiles(string path)
        {
            if (path.Contains("*") || path.Contains("?"))
            {
                var dir = Environment.CurrentDirectory;
                var filename = Path.GetFileName(path);

                if (path.Contains("\\"))
                    dir = Path.GetDirectoryName(path);

                foreach (var file in Directory.GetFiles(dir, filename))
                {
                    yield return file.Replace(Environment.CurrentDirectory + "\\", "");
                }
            }
            else
                yield return path;
        }

        public static void Run(IEnumerable<string> args)
        {
            ContainerBootstrapper.BootstrapStructureMap();

            var application = ObjectFactory.GetInstance<ConsoleApplication>();

            application.SetArguments(args);
            application.Run();
        }
    }
}