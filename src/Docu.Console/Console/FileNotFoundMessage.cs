using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docu.Console
{
    public class BadFileMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "The requested file is in a bad format and could not be loaded!";
        }
    }
}
