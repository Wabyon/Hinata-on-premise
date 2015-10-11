using System;
using System.Linq;
using Hinata.Markdown;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hinata.Search.Models
{
    internal class ItemIndexModel
    {
        public string Id { get; set; }

        public bool IsPublic { get; set; }

        public string Type { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string[] Tags { get; set; }

        public int Revision { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public ItemIndexModel()
        {
            Tags = new string[]{};
        }

        public ItemIndexModel(Item item)
        {
            if (item == null) throw new ArgumentNullException("item");

            Id = item.Id;
            IsPublic = item.IsPublic;
            Type = item.Type.ToString().ToLower();
            Author = item.Author.Name;
            Title = item.Title;
            Tags = item.ItemTags.Select(x => x.Name).ToArray();
            Revision = item.RevisionNo;
            Created = item.CreatedDateTime;
            Modified = item.LastModifiedDateTime;

            using (var parser = new MarkdownParser())
            {
                Body = parser.Strip(item.Body);
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
        }
    }
}
