using System;

namespace Docu.Console
{
    public class Switch : ISwitch
    {
        private readonly string argValue;
        private readonly Func<bool> action;

        public Switch(string argValue, Func<bool> action)
        {
            this.argValue = argValue;
            this.action = action;
        }

        public bool IsMatch(string arg)
        {
            return arg == argValue;
        }

        public bool Handle(string arg)
        {
            return action();
        }
    }
}