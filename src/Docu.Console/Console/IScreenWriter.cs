namespace Docu.Console
{
    public interface IScreenWriter
    {
        void WriteMessage(IScreenMessage message);
        void WriteLine(string message);
    }
}