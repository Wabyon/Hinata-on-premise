using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hinata.Data.Models
{
    internal class DraftRegisterDataModel
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public ItemType Type { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Comment { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public DraftRegisterDataModel(Draft draft)
        {
            Id = draft.Id;
            UserId = draft.Author.Id;
            Type = draft.Type;
            Title = draft.Title;
            Body = draft.Body;
            LastModifiedDateTime = draft.LastModifiedDateTime;
            Comment = draft.Comment;
        }
    }

    internal class DraftSelectDataModel
    {
        public string Id { get; set; }

        public ItemType Type { get; set; }

        public bool ItemIsPublic { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime ItemCreatedDateTime { get; set; }

        public int ItemRevisionCount { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public string Tags { get; set; }

        public string Comment { get; set; }

        public int CurrentRevisionNo { get; set; }

        public Draft ToEntity()
        {
            var draft = new Draft
            {
                Id = Id,
                Title = Title,
                Body = Body,
                Type = Type,
                ItemIsPublic = ItemIsPublic,
                ItemCreatedDateTime = ItemCreatedDateTime,
                ItemRevisionCount = ItemRevisionCount,
                LastModifiedDateTime = LastModifiedDateTime,
                Comment = Comment,
                CurrentRevisionNo = CurrentRevisionNo,
            };

            if (!string.IsNullOrWhiteSpace(Author))
            {
                var xmlAuthor = new XmlDocument();
                xmlAuthor.LoadXml(Author);
                var jsonAuthor = Regex.Replace(JsonConvert.SerializeXmlNode(xmlAuthor), "(?<=\")(@)(?!.*\":\\s )", "",
                    RegexOptions.IgnoreCase);
                var jObjectAuthor = JObject.Parse(jsonAuthor)["Author"];
                var author = JsonConvert.DeserializeObject<User>(jObjectAuthor.ToString());
                draft.Author = author;
            }
            else
            {
                draft.Author = User.Unknown;
            }

            if (string.IsNullOrWhiteSpace(Tags)) return draft;

            var xmlTags = new XmlDocument();
            xmlTags.LoadXml(Tags);
            var jsonTags = Regex.Replace(JsonConvert.SerializeXmlNode(xmlTags), "(?<=\")(@)(?!.*\":\\s )", "", RegexOptions.IgnoreCase);
            var jObjectTags = JObject.Parse(jsonTags)["Tags"]["Tag"];
            if (jObjectTags.Type == JTokenType.Array)
            {
                var tags = JsonConvert.DeserializeObject<IEnumerable<ItemTag>>(jObjectTags.ToString());
                foreach (var tag in tags)
                {
                    draft.ItemTags.Add(tag);
                }
            }
            else
            {
                var tag = JsonConvert.DeserializeObject<ItemTag>(jObjectTags.ToString());
                draft.ItemTags.Add(tag);
            }

            return draft;
        }
    }
}
