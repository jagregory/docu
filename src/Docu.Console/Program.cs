using System.Diagnostics;
using Docu.Console;

namespace Docu
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Debugger.Break();
            ConsoleApplication.Run(args);
        }
    }
}