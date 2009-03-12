using System;
using System.Collections.Generic;
using System.Xml;

namespace DrDoc.Associations
{
    public interface IAssociator
    {
        IList<Association> Examine(Type[] types, XmlNode[] snippets);
    }
}