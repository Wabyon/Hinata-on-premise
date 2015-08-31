using System;

namespace Hinata
{
    public class Item
    {
        private readonly ItemTagCollection _itemTags = new ItemTagCollection();

        public string Id { get; internal set; }

        public bool IsPublic { get; internal set; }

        public ItemType Type { get; internal set; }

        public User Author { get; internal set; }

        public string Title { get; internal set; }

        public string Body { get; internal set; }

        public int CommentCount { get; internal set; }

        public ItemTagCollection ItemTags { get { return _itemTags; } }

        public DateTime CreatedDateTime { get; internal set; }

        public DateTime LastModifiedDateTime { get; set; }

        public int RevisionNo { get; internal set; }

        public int RevisionCount { get; internal set; }

        internal string Comment { get; set; }

        internal Item()
        {
        }

        public Draft ToDraft()
        {
            return new Draft(this)
            {
                CurrentRevisionNo = RevisionNo,
            };
        }

        public Comment NewComment(User user)
        {
            if (user == null) throw new ArgumentNullException("user");

            CommentCount++;

            return new Comment(this)
            {
                User = user,
                CreatedDateTime = DateTime.Now,
                LastModifiedDateTime = DateTime.Now
            };
        }
    }
}
