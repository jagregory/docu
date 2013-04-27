namespace Docu.Console
{
    using System.Collections.Generic;

    public class SplashMessage : IScreenMessage
    {
        public IEnumerable<string> GetBody()
        {
            yield return "-------------------------------";
            yield return " docu: simple docs done simply ";
            yield return "-------------------------------";
            yield return string.Empty;
        }
    }
}