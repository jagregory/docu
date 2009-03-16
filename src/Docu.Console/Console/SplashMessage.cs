using System.Collections.Generic;

namespace Docu.Console
{
    public class SplashMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "-------------------------------";
            yield return " docu: simple docs done simply ";
            yield return "-------------------------------";
            yield return "";
        }
    }
}