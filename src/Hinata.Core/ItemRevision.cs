using System;

namespace Hinata
{
    public class ItemRevision
    {
        private readonly TagCollection _tags = new TagCollection();

        public string ItemId { get; internal set; }

        public int RevisionNo { get; internal set; }

        public DateTime ModifiedDateTime { get; internal set; }

        public string Comment { get; internal set; }

        public string Title { get; internal set; }

        public TagCollection Tags { get { return _tags; } }

        public string Body { get; internal set; }

        public bool IsFirst { get; internal set; }

        public bool IsCurrent { get; internal set; }
    }
}
