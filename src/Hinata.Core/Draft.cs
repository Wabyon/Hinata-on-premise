using System;

namespace Hinata
{
    public class Draft
    {
        private readonly Item _item;

        public bool IsContributed
        {
            get { return (_item.CreatedDateTime != DateTime.MinValue); }
        }

        public string Id
        {
            get { return _item.Id; }
            internal set { _item.Id = value; }
        }

        public int CurrentRevisionNo { get; internal set; }

        public string Comment { get; set; }

        public ItemType Type
        {
            get { return _item.Type; }
            set { _item.Type = value; }
        }

        public bool ItemIsPublic
        {
            get { return _item.IsPublic; }
            set { _item.IsPublic = value; }
        }

        public string Title
        {
            get { return _item.Title; }
            set { _item.Title = value; }
        }

        public string Body
        {
            get { return _item.Body; }
            set { _item.Body = value; }
        }

        public User Author
        {
            get { return _item.Author; }
            set { _item.Author = value; }
        }

        internal DateTime? ItemCreatedDateTime
        {
            get { return IsContributed ? _item.CreatedDateTime : (DateTime?)null; }
            set
            {
                if (value.HasValue) _item.CreatedDateTime = value.Value;
            }
        }

        public DateTime LastModifiedDateTime
        {
            get { return _item.LastModifiedDateTime; }
            set { _item.LastModifiedDateTime = value; }
        }

        public ItemTagCollection ItemTags
        {
            get { return _item.ItemTags; }
        }

        internal int ItemRevisionCount
        {
            get { return _item.RevisionCount; }
            set { _item.RevisionCount = value; }
        }

        internal Draft() : this(new Item())
        {
        }

        internal Draft(Item item)
        {
            _item = item;
            CurrentRevisionNo = -1;
        }

        public static Draft NewDraft(User author, ItemType type)
        {
            var draft = new Draft
            {
                Id = CreateNewId(),
                Author = author,
                Type = type,
                LastModifiedDateTime = DateTime.Now,
            };

            return draft;
        }

        public Item ToItem()
        {
            return ToItem(_item.IsPublic);
        }

        public Item ToItem(bool isPublic)
        {
            if (!IsContributed)
            {
                _item.CreatedDateTime = DateTime.Now;
            }
            _item.IsPublic = isPublic;
            _item.LastModifiedDateTime = DateTime.Now;
            _item.Comment = Comment;
            _item.RevisionNo = CurrentRevisionNo + 1;

            return _item;
        }

        private static string CreateNewId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
