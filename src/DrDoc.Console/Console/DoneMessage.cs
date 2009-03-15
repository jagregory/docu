using System.Collections.Generic;

namespace DrDoc.Console
{
    public class DoneMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "";
            yield return "Generation complete";
        }
    }
}