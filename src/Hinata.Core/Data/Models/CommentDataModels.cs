using System;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hinata.Data.Models
{
    internal class CommentRegisterDataModel
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string ItemId { get; set; }

        public string Body { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public CommentRegisterDataModel(Comment comment)
        {
            Id = comment.Id;
            UserId = comment.User.Id;
            ItemId = comment.ItemId;
            Body = comment.Body;
            CreatedDateTime = comment.CreatedDateTime;
            LastModifiedDateTime = comment.LastModifiedDateTime;
        }
    }

    internal class CommentSelectDataModel
    {
        public string Id { get; set; }

        public string ItemId { get; set; }

        public string User { get; set; }

        public string Body { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public Comment ToEntity()
        {
            var comment = new Comment
            {
                Id = Id,
                ItemId = ItemId,
                Body = Body,
                CreatedDateTime = CreatedDateTime,
                LastModifiedDateTime = LastModifiedDateTime,
            };

            if (!string.IsNullOrWhiteSpace(User))
            {
                var xmlUser = new XmlDocument();
                xmlUser.LoadXml(User);
                var jsonUser = Regex.Replace(JsonConvert.SerializeXmlNode(xmlUser), "(?<=\")(@)(?!.*\":\\s )", "",
                    RegexOptions.IgnoreCase);
                var jObjectUser = JObject.Parse(jsonUser)["User"];
                var user = JsonConvert.DeserializeObject<User>(jObjectUser.ToString());
                comment.User = user;
            }
            else
            {
                comment.User = Hinata.User.Unknown;
            }

            return comment;
        }
    }
}
