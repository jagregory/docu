using System.Xml;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Parsing.Comments
{
    public class SeeCodeCommentParser : ICommentNodeParser
    {
        public bool CanParse(XmlNode node)
        {
            return node.Name == "see";
        }

        public Comment Parse(ICommentParser parser, XmlNode node, bool first, bool last, ParseOptions options)
        {
            IReferencable reference = new NullReference();
            if (node.Attributes["cref"] == null) return new See(reference);
            var referenceTarget = IdentifierFor.XmlString(node.Attributes["cref"].Value);

            if (referenceTarget is NamespaceIdentifier)
                reference = Namespace.Unresolved((NamespaceIdentifier)referenceTarget);
            else if (referenceTarget is TypeIdentifier)
                reference = DeclaredType.Unresolved((TypeIdentifier)referenceTarget, Namespace.Unresolved(referenceTarget.CloneAsNamespace()));
            else if (referenceTarget is MethodIdentifier)
                reference = Method.Unresolved(
                    (MethodIdentifier)referenceTarget,
                    DeclaredType.Unresolved(
                        referenceTarget.CloneAsType(),
                        Namespace.Unresolved(referenceTarget.CloneAsNamespace())
                    )
                );
            else if (referenceTarget is PropertyIdentifier)
                reference = Property.Unresolved(
                    (PropertyIdentifier)referenceTarget,
                    DeclaredType.Unresolved(
                        referenceTarget.CloneAsType(),
                        Namespace.Unresolved(referenceTarget.CloneAsNamespace())
                    )
                );
            else if (referenceTarget is EventIdentifier)
                reference = Event.Unresolved(
                    (EventIdentifier)referenceTarget,
                    DeclaredType.Unresolved(
                        referenceTarget.CloneAsType(),
                        Namespace.Unresolved(referenceTarget.CloneAsNamespace())
                    )
                );
            else if (referenceTarget is FieldIdentifier)
                reference = Field.Unresolved(
                    (FieldIdentifier)referenceTarget,
                    DeclaredType.Unresolved(
                        referenceTarget.CloneAsType(),
                        Namespace.Unresolved(referenceTarget.CloneAsNamespace())
                    )
                );

            return new See(reference);
        }
    }
}