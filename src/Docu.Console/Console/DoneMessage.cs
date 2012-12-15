namespace Docu.Console
{
    using System.Collections.Generic;

    public class DoneMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "";
            yield return "Generation complete";
        }
    }
}