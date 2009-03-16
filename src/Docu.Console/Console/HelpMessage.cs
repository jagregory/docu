using System.Collections.Generic;

namespace Docu.Console
{
    public class HelpMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "usuage: docu assembly [assembly...] [xml file...]";
        }
    }
}