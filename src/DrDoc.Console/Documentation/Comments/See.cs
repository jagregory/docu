using DrDoc.Documentation;
using DrDoc.Documentation.Comments;

namespace DrDoc.Documentation.Comments
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