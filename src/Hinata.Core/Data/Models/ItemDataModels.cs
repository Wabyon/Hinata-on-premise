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

        public string CreateUserId { get; set; }

        public string LastModifyUserId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public DateTime? PublishSince { get; set; }

        public DateTime? PublishUntil { get; set; }

        public ItemRegisterDataModel(Item item)
        {
            Id = item.Id;
            RevisionNo = item.RevisionNo;
            Type = item.Type;
            IsPublic = item.IsPublic;
            CreateUserId = item.Author.Id;
            LastModifyUserId = item.Editor.Id;
            Title = item.Title;
            Body = item.Body;
            Comment = item.Comment;
            CreatedDateTime = item.CreatedDateTime;
            LastModifiedDateTime = item.LastModifiedDateTime;
            PublishSince = item.PublishSince;
            PublishUntil = item.PublishUntil;
        }
    }

    internal class ItemSelectDataModel
    {
        public string Id { get; set; }

        public ItemType Type { get; set; }

        public bool IsPublic { get; set; }

        public string Author { get; set; }

        public string Editor { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public string Tags { get; set; }

        public string Collaborators { get; set; }

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
                var jsonAuthor = Regex.Replace(JsonConvert.SerializeXmlNode(xmlAuthor), "(?<=\")(@)(?!.*\":\\s )",
                    "",
                    RegexOptions.IgnoreCase);
                var jObjectAuthor = JObject.Parse(jsonAuthor)["Author"];
                var author = JsonConvert.DeserializeObject<User>(jObjectAuthor.ToString());
                item.Author = author;
            }
            else
            {
                item.Author = User.Unknown;
            }

            if (!string.IsNullOrWhiteSpace(Editor))
            {
                var xmlEditor = new XmlDocument();
                xmlEditor.LoadXml(Editor);
                var jsonEditor = Regex.Replace(JsonConvert.SerializeXmlNode(xmlEditor), "(?<=\")(@)(?!.*\":\\s )",
                    "",
                    RegexOptions.IgnoreCase);
                var jObjectEditor = JObject.Parse(jsonEditor)["Editor"];
                var editor = JsonConvert.DeserializeObject<User>(jObjectEditor.ToString());
                item.Editor = editor;
            }
            else
            {
                item.Editor = User.Unknown;
            }

            if (!string.IsNullOrWhiteSpace(Tags))
            {
                var xmlTags = new XmlDocument();
                xmlTags.LoadXml(Tags);
                var jsonTags = Regex.Replace(JsonConvert.SerializeXmlNode(xmlTags), "(?<=\")(@)(?!.*\":\\s )", "",
                    RegexOptions.IgnoreCase);
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
            }

            if (!string.IsNullOrWhiteSpace(Collaborators))
            {

                var xmlCollaborators = new XmlDocument();
                xmlCollaborators.LoadXml(Collaborators);
                var jsonCollaborators = Regex.Replace(JsonConvert.SerializeXmlNode(xmlCollaborators),
                    "(?<=\")(@)(?!.*\":\\s )", "", RegexOptions.IgnoreCase);
                var jObjectCollaborators = JObject.Parse(jsonCollaborators)["Collaborators"]["Collaborator"];
                if (jObjectCollaborators.Type == JTokenType.Array)
                {
                    var collaborators = JsonConvert.DeserializeObject<IEnumerable<Collaborator>>(jObjectCollaborators.ToString());
                    foreach (var collaborator in collaborators)
                    {
                        item.AddCollaborator(collaborator);
                    }
                }
                else
                {
                    var collaborator = JsonConvert.DeserializeObject<Collaborator>(jObjectCollaborators.ToString());
                    item.AddCollaborator(collaborator);
                }
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

        public string Author { get; set; }

        public string Editor { get; set; }

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

            if (!string.IsNullOrWhiteSpace(Author))
            {
                var xmlAuthor = new XmlDocument();
                xmlAuthor.LoadXml(Author);
                var jsonAuthor = Regex.Replace(JsonConvert.SerializeXmlNode(xmlAuthor), "(?<=\")(@)(?!.*\":\\s )",
                    "",
                    RegexOptions.IgnoreCase);
                var jObjectAuthor = JObject.Parse(jsonAuthor)["Author"];
                var author = JsonConvert.DeserializeObject<User>(jObjectAuthor.ToString());
                itemRevision.Author = author;
            }
            else
            {
                itemRevision.Author = User.Unknown;
            }

            if (!string.IsNullOrWhiteSpace(Editor))
            {
                var xmlEditor = new XmlDocument();
                xmlEditor.LoadXml(Editor);
                var jsonEditor = Regex.Replace(JsonConvert.SerializeXmlNode(xmlEditor), "(?<=\")(@)(?!.*\":\\s )",
                    "",
                    RegexOptions.IgnoreCase);
                var jObjectEditor = JObject.Parse(jsonEditor)["Editor"];
                var editor = JsonConvert.DeserializeObject<User>(jObjectEditor.ToString());
                itemRevision.Editor = editor;
            }
            else
            {
                itemRevision.Editor = User.Unknown;
            }

            if (!string.IsNullOrWhiteSpace(Tags))
            {
                var xmlTags = new XmlDocument();
                xmlTags.LoadXml(Tags);
                var jsonTags = Regex.Replace(JsonConvert.SerializeXmlNode(xmlTags), "(?<=\")(@)(?!.*\":\\s )", "",
                    RegexOptions.IgnoreCase);
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
            }

            return itemRevision;
        }
    }
}
