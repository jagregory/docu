namespace Docu.Console
{
    public interface ISwitch
    {
        bool IsMatch(string arg);

        /// <summary>
        /// Handle the argument
        /// </summary>
        /// <param name="arg">argument</param>
        /// <returns>Continue/true or exit/false</returns>
        bool Handle(string arg);
    }
}