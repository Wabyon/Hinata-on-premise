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

        public string Title { get; set; }

        public string Body { get; set; }

        public string Comment { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public DraftRegisterDataModel(Draft draft)
        {
            Id = draft.Id;
            UserId = draft.Editor.Id;
            Title = draft.Title;
            Body = draft.Body;
            LastModifiedDateTime = draft.LastModifiedDateTime;
            Comment = draft.Comment;
        }
    }

    internal class DraftSelectDataModel
    {
        public string Id { get; set; }

        public bool ItemIsPublic { get; set; }

        public string Author { get; set; }

        public string Editor { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string PublishedBody { get; set; }

        public DateTime ItemCreatedDateTime { get; set; }

        public int ItemRevisionCount { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public string Tags { get; set; }

        public string Collaborators { get; set; }

        public string Comment { get; set; }

        public int CurrentRevisionNo { get; set; }

        public Draft ToEntity()
        {
            var draft = new Draft
            {
                Id = Id,
                Title = Title,
                Body = Body,
                PublishedBody = PublishedBody,
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

            if (!string.IsNullOrWhiteSpace(Editor))
            {
                var xmlEditor = new XmlDocument();
                xmlEditor.LoadXml(Editor);
                var jsonEditor = Regex.Replace(JsonConvert.SerializeXmlNode(xmlEditor), "(?<=\")(@)(?!.*\":\\s )", "",
                    RegexOptions.IgnoreCase);
                var jObjectEditor = JObject.Parse(jsonEditor)["Editor"];
                var editor = JsonConvert.DeserializeObject<User>(jObjectEditor.ToString());
                draft.Editor = editor;
            }
            else
            {
                draft.Editor = User.Unknown;
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
                        draft.ItemTags.Add(tag);
                    }
                }
                else
                {
                    var tag = JsonConvert.DeserializeObject<ItemTag>(jObjectTags.ToString());
                    draft.ItemTags.Add(tag);
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
                        draft.AddCollaborator(collaborator);
                    }
                }
                else
                {
                    var collaborator = JsonConvert.DeserializeObject<Collaborator>(jObjectCollaborators.ToString());
                    draft.AddCollaborator(collaborator);
                }
            }

            return draft;
        }
    }
}
