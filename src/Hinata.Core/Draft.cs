using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Hinata
{
    public class Draft
    {
        private readonly Item _item;
        private readonly ItemTagCollection _itemTags = new ItemTagCollection();

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

        public bool ItemIsPublic { get; internal set; }

        public bool ItemIsFreeEditable { get; internal set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string PublishedBody { get; internal set; }

        public User Author
        {
            get { return _item.Author; }
            set { _item.Author = value; }
        }

        /// <summary>編集者</summary>
        public User Editor { get; set; }

        public IReadOnlyCollection<Collaborator> Collaborators
        {
            get { return _item.Collaborators; }
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
            get { return _itemTags; }
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
                Editor = author,
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
            return ToItem(isPublic, _item.IsFreeEditable);
        }

        public Item ToItem(bool isPublic, bool isFreeEditable)
        {
            if (!IsContributed)
            {
                _item.CreatedDateTime = DateTime.Now;
            }

            _item.Title = Title;
            _item.Body = Body;
            _item.Editor = Editor;
            _item.IsPublic = isPublic;
            _item.IsFreeEditable = isFreeEditable;
            _item.LastModifiedDateTime = DateTime.Now;
            _item.Comment = Comment;
            _item.RevisionNo = CurrentRevisionNo + 1;

            _item.ItemTags.Clear();
            _item.ItemTags.AddRange(_itemTags);

            return _item;
        }

        private static string CreateNewId()
        {
            return Guid.NewGuid().ToString("N");
        }

        internal void AddCollaborator(Collaborator collaborator)
        {
            if (collaborator == null) throw new ArgumentNullException("collaborator");

            _item.AddCollaborator(collaborator);
        }
    }
}
