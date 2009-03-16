using System.Collections.Generic;

namespace Docu.Console
{
    public class ProcessingArgumentsMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "Processing arguments";
        }
    }
}