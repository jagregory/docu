namespace Docu.Console
{
    using System.Collections.Generic;

    public class ProcessingArgumentsMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "Processing arguments";
        }
    }
}