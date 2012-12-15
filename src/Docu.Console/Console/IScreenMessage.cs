namespace Docu.Console
{
    using System.Collections.Generic;

    public interface IScreenMessage
    {
        IEnumerable<string> GetBody();
    }
}