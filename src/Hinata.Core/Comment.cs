using System;

namespace Hinata
{
    public class Comment
    {
        public string ItemId { get; internal set; }

        public string Id { get; internal set; }

        public string Body { get; set; }

        public User User { get; internal set; }

        public DateTime CreatedDateTime { get; internal set; }

        public DateTime LastModifiedDateTime { get; set; }

        internal Comment()
        {
        }

        internal Comment(Item item)
        {
            Id = CreateNewId();
            ItemId = item.Id;
        }

        private static string CreateNewId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
