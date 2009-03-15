using System.Collections.Generic;

namespace DrDoc.Console
{
    public class ProcessingArgumentsMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "Processing arguments";
        }
    }
}