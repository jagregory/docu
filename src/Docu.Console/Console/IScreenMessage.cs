using System.Collections.Generic;

namespace Docu.Console
{
    public interface IScreenMessage
    {
        IEnumerable<string> GetBody();
    }
}