using System;
using Docu.Parsing;

namespace Docu
{
    public class GenerationEventArgs : EventArgs
    {
        public GenerationEventArgs(string message, WarningType warningType)
        {
            Message = message;
            WarningType = warningType;
        }

        public WarningType WarningType { get; private set; }
        public string Message { get; private set; }
    }
}