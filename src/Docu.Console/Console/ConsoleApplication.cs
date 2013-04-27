using Docu.Documentation;
using Docu.Events;
using Docu.IO;
using Docu.Output;
using Docu.Parsing;
using Docu.Parsing.Comments;
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
        readonly IScreenWriter screenWriter;
        readonly IList<ISwitch> switches = new List<ISwitch>();
        bool canRun;

        public ConsoleApplication(IScreenWriter screenWriter, DocumentationGenerator documentationGenerator, IEventAggregator eventAggregator)
        {
            this.screenWriter = screenWriter;
            this.documentationGenerator = documentationGenerator;
            this.eventAggregator = eventAggregator;

            WireUpListeners();
            DefineSwitches();
        }

        public static void Run(IEnumerable<string> args)
        {
            var eventAggregator = new EventAggregator();

            var commentParser = new CommentParser(new ICommentNodeParser[]
                {
                    new InlineCodeCommentParser(),
                    new InlineListCommentParser(),
                    new InlineTextCommentParser(),
                    new MultilineCodeCommentParser(),
                    new ParagraphCommentParser(),
                    new ParameterReferenceParser(),
                    new SeeCodeCommentParser(),
                });

            var documentModel = new DocumentModel(commentParser, eventAggregator);
            var pageWriter = new BulkPageWriter(new PageWriter(new HtmlGenerator(), new FileSystemOutputWriter(), new PatternTemplateResolver()));
            var generator = new DocumentationGenerator(new AssemblyLoader(), new XmlLoader(), new AssemblyXmlParser(documentModel), pageWriter, new UntransformableResourceManager(), eventAggregator);
            var application = new ConsoleApplication(new ConsoleScreenWriter(), generator, eventAggregator);

            application.SetArguments(args);
            application.Run();
        }

        public void Run()
        {
            if (!canRun)
            {
                ShowMessage(Messages.Help);
                return;
            }

            if (ProcessSwitches() == false)
            {
                return;
            }

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

        void SetArguments(IEnumerable<string> args)
        {
            arguments.AddRange(args);

            if (arguments.Count > 0)
            {
                canRun = true;
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

        void BadFile(string path)
        {
            ShowMessage(new BadFileMessage(path));
        }

        void DefineSwitches()
        {
            switches.Add(
                new Switch(
                    "--help",
                    () =>
                        {
                            ShowMessage(new HelpMessage());
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

        void ShowMessage(IScreenMessage message)
        {
            screenWriter.WriteMessage(message);
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

                ShowMessage(new InvalidArgumentMessage(argument));
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

        bool VerifyXmls(IEnumerable<string> xmls)
        {
            if (!xmls.Any())
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

        void Warning(string message)
        {
            ShowMessage(new WarningMessage(message));
        }

        void WireUpListeners()
        {
            eventAggregator.GetEvent<WarningEvent>().Subscribe(Warning);
            eventAggregator.GetEvent<BadFileEvent>().Subscribe(BadFile);
        }
    }
}