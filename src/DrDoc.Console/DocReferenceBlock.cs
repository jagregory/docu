namespace DrDoc
{
    public class DocReferenceBlock : DocBlock
    {
        public DocReferenceBlock(IReferencable reference)
        {
            Reference = reference;
        }

        public IReferencable Reference { get; set; }
    }
}