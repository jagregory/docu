using DrDoc.Associations;

namespace DrDoc
{
    public class ExternalReference : IReferencable
    {
        public ExternalReference(MemberName name)
        {
            Name = name;
            FullName = Name.ToString();

            if (Name is TypeMemberName)
                FullName = Name.CloneAsNamespace() + "." + Name;
        }

        public MemberName Name { get; private set; }
        public string FullName { get; private set; }
    }
}