using System;

namespace Hinata
{
    public class ItemRevision
    {
        private readonly ItemTagCollection _itemTags = new ItemTagCollection();

        public string ItemId { get; internal set; }

        public int RevisionNo { get; internal set; }

        public DateTime ModifiedDateTime { get; internal set; }

        public string Comment { get; internal set; }

        public string Title { get; internal set; }

        public ItemTagCollection ItemTags { get { return _itemTags; } }

        public string Body { get; internal set; }

        public bool IsFirst { get; internal set; }

        public bool IsCurrent { get; internal set; }
    }
}
