namespace Docu.Console
{
    using System.Collections.Generic;

    public class StartMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "Starting documentation generation";
        }
    }
}