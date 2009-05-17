using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IResolvable
    {
        bool IsResolved { get; }
        void Resolve(IDictionary<Identifier, IReferencable> referencables);
    }

    public interface IReferencable : IResolvable
    {
        string Name { get; }
        string FullName { get; }
        string PrettyName { get; }
        bool IsExternal { get; }
        bool IsIdentifiedBy(Identifier otherIdentifier);
        void ConvertToExternalReference();
    }

    public class NullReference : IReferencable
    {
        public bool IsResolved
        {
            get { return true; }
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
        }

        public string Name
        {
            get { return string.Empty; }
        }

        public string FullName
        {
            get { return string.Empty; }
        }

        public string PrettyName
        {
            get { return string.Empty; }
        }

        public bool IsExternal
        {
            get { return true; }
        }

        public bool IsIdentifiedBy(Identifier otherIdentifier)
        {
            return false;
        }

        public void ConvertToExternalReference()
        {
            
        }
    }
}