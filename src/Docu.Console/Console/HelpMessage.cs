namespace Docu.Console
{
    using System.Collections.Generic;

    public class HelpMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "usuage: docu pattern.dll [pattern.dll ...] [pattern.xml ...]";
            yield return string.Empty;
            yield return " * One or more dll matching patterns can be specified,";
            yield return "   e.g. MyProject*.dll or MyProject.dll";
            yield return string.Empty;
            yield return " * Xml file names can be inferred from the dll patterns";
            yield return "   or can be explicitly named.";
            yield return string.Empty;
            yield return "Switches:";
            yield return "  --help          Shows this message";
            yield return "  --output=value  Sets the output path to value";
        }
    }
}