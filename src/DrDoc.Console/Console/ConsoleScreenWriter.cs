namespace DrDoc.Console
{
    public class ConsoleScreenWriter : IScreenWriter
    {
        public void WriteMessage(IScreenMessage message)
        {
            foreach (var line in message.GetBody())
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