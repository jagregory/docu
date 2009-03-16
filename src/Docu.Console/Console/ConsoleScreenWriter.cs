namespace Docu.Console
{
    public class ConsoleScreenWriter : IScreenWriter
    {
        public void WriteMessage(IScreenMessage message)
        {
            foreach (string line in message.GetBody())
            {
                WriteLine(line);
            }
        }

        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}