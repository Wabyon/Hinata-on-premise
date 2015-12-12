using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hinata.Data.Models
{
    internal class LikeSelectDataModel
    {
        public string Id { get; set; }
        public string ItemId { get; set; }
        public string UserId { get; set; }

        public Like ToEntity()
        {
            var like = new Like
            {
                Id = Id,
                ItemId = ItemId,
                UserId = UserId,
            };

            return like;
        }
    }

    internal class LikeAddDataModel
    {
        public string Id { get; set; }
        public string ItemId { get; set; }
        public string UserId { get; set; }

        public LikeAddDataModel(Like like)
        {
            Id = like.Id;
            ItemId = like.ItemId;
            UserId = like.UserId;
        }
    }
}
