using System;
using System.Collections.Generic;
using Hinata.Collections;
using Hinata.Exceptions;

namespace Hinata
{
    public class Like
    {
        public string Id { get; internal set; }

        public string ItemId { get; internal set; }

        public string UserId { get; internal set; }

        internal Like()
        {
        }

        internal Like(Item item)
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
