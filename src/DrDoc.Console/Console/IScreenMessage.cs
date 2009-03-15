using System.Collections.Generic;

namespace DrDoc.Console
{
    public interface IScreenMessage
    {
        IEnumerable<string> GetBody();
    }
}