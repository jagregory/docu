using System.Collections.Generic;

namespace DrDoc.Console
{
    public class StartMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "Starting documentation generation";
        }
    }
}