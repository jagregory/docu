namespace Docu.Console
{
    using System;

    public class ParameterSwitch : ISwitch
    {
        private readonly string argValue;
        private readonly Func<string, bool> action;

        public ParameterSwitch(string argValue, Func<string, bool> action)
        {
            this.argValue = argValue;
            this.action = action;
        }

        public bool IsMatch(string arg)
        {
            return arg.StartsWith(argValue) && arg.Contains("=");
        }

        public bool Handle(string arg)
        {
            var value = arg.Split('=')[1];

            return action(value);
        }
    }
}