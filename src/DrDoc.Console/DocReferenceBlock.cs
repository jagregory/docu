namespace DrDoc
{
    public class DocReferenceBlock : DocBlock, IReferrer
    {
        public DocReferenceBlock(IReferencable reference)
        {
            Reference = reference;
        }

        public IReferencable Reference { get; set; }
    }
}