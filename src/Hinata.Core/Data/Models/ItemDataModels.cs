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

        public bool IsPublic { get; set; }

        public ItemType Type { get; set; }

        public string UserId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public ItemRegisterDataModel(Item item)
        {
            Id = item.Id;
            Type = item.Type;
            IsPublic = item.IsPublic;
            UserId = item.Author.Id;
            Title = item.Title;
            Body = item.Body;
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
                CommentCount = CommentCount
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
                var tags = JsonConvert.DeserializeObject<IEnumerable<Tag>>(jObjectTags.ToString());
                foreach (var tag in tags)
                {
                    item.Tags.Add(tag);
                }
            }
            else
            {
                var tag = JsonConvert.DeserializeObject<Tag>(jObjectTags.ToString());
                item.Tags.Add(tag);
            }

            return item;
        }
    }
}
