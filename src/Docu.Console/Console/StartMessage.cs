using System.Collections.Generic;

namespace Docu.Console
{
    public class StartMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "Starting documentation generation";
        }
    }
}