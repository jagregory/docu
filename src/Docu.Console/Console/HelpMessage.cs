using System.Collections.Generic;

namespace Docu.Console
{
    public class HelpMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "usage: docu pattern.dll [pattern.dll ...] [pattern.xml ...]";
            yield return "";
            yield return " * One or more dll matching patterns can be specified,";
            yield return "   e.g. MyProject*.dll or MyProject.dll";
            yield return "";
            yield return " * Xml file names can be inferred from the dll patterns";
            yield return "   or can be explicitly named.";
            yield return "";
            yield return "Switches:";
            yield return "  --help          Shows this message";
            yield return "  --output=value  Sets the output path to value";
        }
    }
}
