using System.Collections.Generic;

namespace Docu.Console
{
    public class DocumentModelWarningMessage : IScreenMessage
    {
        private readonly string message;

        public DocumentModelWarningMessage(string message)
        {
            this.message = message;
        }

        public IEnumerable<string> GetBody()
        {
            yield return "WARNING: " + message;
        }
    }
}