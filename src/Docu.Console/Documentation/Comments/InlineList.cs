namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public abstract class InlineList : Comment
    {
        protected InlineList()
        {
            Items = new List<InlineListItem>();
        }

        public IList<InlineListItem> Items { get; set; }
    }

    public class DefinitionList : InlineList
    {
    }

    public class BulletList : InlineList
    {
    }

    public class NumberList : InlineList
    {
    }

    public class TableList : InlineList
    {
    }

    public class InlineListItem
    {
        public InlineListItem(Paragraph term, Paragraph definition)
        {
            Term = term;
            Definition = definition;
        }

        public Paragraph Term { get; set; }
        public Paragraph Definition { get; set; }
    }
}
