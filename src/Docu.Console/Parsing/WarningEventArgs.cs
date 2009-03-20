using System;

namespace Docu.Parsing
{
    public class DocumentModelWarningEventArgs : EventArgs
    {
        public DocumentModelWarningEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class ParserWarningEventArgs : EventArgs
    {
        public ParserWarningEventArgs(string message, WarningType type)
        {
            Message = message;
            WarningType = type;
        }

        public string Message { get; private set; }
        public WarningType WarningType { get; private set; }
    }

    public enum WarningType
    {
        DocumentModel,
        Unknown
    }
}