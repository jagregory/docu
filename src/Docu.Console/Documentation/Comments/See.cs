namespace Docu.Documentation.Comments
{
    public class See : IComment, IReferrer
    {
        public See(IReferencable reference)
        {
            Reference = reference;
        }

        public IReferencable Reference { get; set; }
    }
}