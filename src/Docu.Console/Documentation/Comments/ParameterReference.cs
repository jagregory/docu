namespace Docu.Documentation.Comments
{
    public class ParameterReference : BaseComment
    {
        public ParameterReference(string parameter)
        {
            Parameter = parameter;
        }

        public string Parameter { get; private set; }

        public override string ToString()
        {
            return Parameter;
        }
    }
}