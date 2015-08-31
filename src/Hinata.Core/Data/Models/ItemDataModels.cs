using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hinata.Data.Models
{
    internal class ItemRegisterDataModel
    {
        public string Id { get; set; }

        public int RevisionNo { get; set; }

        public bool IsPublic { get; set; }

        public ItemType Type { get; set; }

        public string UserId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public ItemRegisterDataModel(Item item)
        {
            Id = item.Id;
            RevisionNo = item.RevisionNo;
            Type = item.Type;
            IsPublic = item.IsPublic;
            UserId = item.Author.Id;
            Title = item.Title;
            Body = item.Body;
            Comment = item.Comment;
            CreatedDateTime = item.CreatedDateTime;
            LastModifiedDateTime = item.LastModifiedDateTime;
        }
    }

    internal class ItemSelectDataModel
    {
        public string Id { get; set; }

        public ItemType Type { get; set; }

        public bool IsPublic { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public string Tags { get; set; }

        public int CommentCount { get; set; }

        public int RevisionCount { get; set; }

        public int RevisionNo { get; set; }

        public Item ToEntity()
        {
            var item = new Item
            {
                Id = Id,
                Title = Title,
                Body = Body,
                Type = Type,
                IsPublic = IsPublic,
                CreatedDateTime = CreatedDateTime,
                LastModifiedDateTime = LastModifiedDateTime,
                CommentCount = CommentCount,
                RevisionCount = RevisionCount,
                RevisionNo = RevisionNo,
            };

            if (!string.IsNullOrWhiteSpace(Author))
            {
                var xmlAuthor = new XmlDocument();
                xmlAuthor.LoadXml(Author);
                var jsonAuthor = Regex.Replace(JsonConvert.SerializeXmlNode(xmlAuthor), "(?<=\")(@)(?!.*\":\\s )", "",
                    RegexOptions.IgnoreCase);
                var jObjectAuthor = JObject.Parse(jsonAuthor)["Author"];
                var author = JsonConvert.DeserializeObject<User>(jObjectAuthor.ToString());
                item.Author = author;
            }
            else
            {
                item.Author = User.Unknown;
            }

            if (string.IsNullOrWhiteSpace(Tags)) return item;

            var xmlTags = new XmlDocument();
            xmlTags.LoadXml(Tags);
            var jsonTags = Regex.Replace(JsonConvert.SerializeXmlNode(xmlTags), "(?<=\")(@)(?!.*\":\\s )", "", RegexOptions.IgnoreCase);
            var jObjectTags = JObject.Parse(jsonTags)["Tags"]["Tag"];
            if (jObjectTags.Type == JTokenType.Array)
            {
                var tags = JsonConvert.DeserializeObject<IEnumerable<ItemTag>>(jObjectTags.ToString());
                foreach (var tag in tags)
                {
                    item.ItemTags.Add(tag);
                }
            }
            else
            {
                var tag = JsonConvert.DeserializeObject<ItemTag>(jObjectTags.ToString());
                item.ItemTags.Add(tag);
            }

            return item;
        }
    }

    internal class ItemRevisionSelectDataModel
    {
        public string ItemId { get; set; }

        public int RevisionNo { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        public string Comment { get; set; }

        public string Title { get; set; }

        public string Tags { get; set; }

        public string Body { get; set; }

        public bool IsFirst { get; set; }

        public bool IsCurrent { get; set; }

        public ItemRevision ToEntity()
        {
            var itemRevision = new ItemRevision
            {
                ItemId = ItemId,
                RevisionNo = RevisionNo,
                ModifiedDateTime = ModifiedDateTime,
                Comment = Comment,
                Title = Title,
                Body = Body,
                IsFirst = IsFirst,
                IsCurrent = IsCurrent,
            };

            if (string.IsNullOrWhiteSpace(Tags)) return itemRevision;

            var xmlTags = new XmlDocument();
            xmlTags.LoadXml(Tags);
            var jsonTags = Regex.Replace(JsonConvert.SerializeXmlNode(xmlTags), "(?<=\")(@)(?!.*\":\\s )", "", RegexOptions.IgnoreCase);
            var jObjectTags = JObject.Parse(jsonTags)["Tags"]["Tag"];
            if (jObjectTags.Type == JTokenType.Array)
            {
                var tags = JsonConvert.DeserializeObject<IEnumerable<ItemTag>>(jObjectTags.ToString());
                foreach (var tag in tags)
                {
                    itemRevision.ItemTags.Add(tag);
                }
            }
            else
            {
                var tag = JsonConvert.DeserializeObject<ItemTag>(jObjectTags.ToString());
                itemRevision.ItemTags.Add(tag);
            }

            return itemRevision;
        }
    }
}
