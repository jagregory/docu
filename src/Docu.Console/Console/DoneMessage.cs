using System.Collections.Generic;

namespace Docu.Console
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