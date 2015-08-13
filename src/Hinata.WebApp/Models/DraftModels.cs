using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Hinata.Web.Mvc.DataAnnotations;

namespace Hinata.Models
{
    public class DraftIndexModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public bool UnTitled { get; set; }

        public bool IsContributed { get; set; }

        public DateTime LastModifiedDateTime { get; set; }
    }

    public class DraftEditModel : IValidatableObject
    {
        public string Id { get; set; }

        public ItemType ItemType { get; set; }

        [AllowHtml]
        [PlaceHolder("タイトル")]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "本文")]
        public string Body { get; set; }

        [ReadOnly(true)]
        public string Html { get; set; }

        [PlaceHolder("タグをスペース区切りで入力　例）T-SQL SQLServer[2012]")]
        public string TagInlineString { get; set; }

        [Display(Name = "限定共有")]
        public bool ItemIsPrivate { get; set; }

        public EntryMode EntryMode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (EntryMode == EntryMode.SaveDraft && string.IsNullOrWhiteSpace(TagInlineString)) return validationResults;

            if (string.IsNullOrWhiteSpace(Title))
            {
                validationResults.Add(new ValidationResult("タイトルは必須です。", new[] { "Title" }));
            }

            if (string.IsNullOrWhiteSpace(TagInlineString))
            {
                validationResults.Add(new ValidationResult("タグは必須です。", new[] {"TagInlineString"}));
            }
            else
            {
                var tagNames = CreateTagCollectionFromInlineText().Select(x => x.Name).ToArray();
                var cnt = tagNames.Count();
                var distinctCnt = tagNames.Distinct().Count();
                if (cnt != distinctCnt)
                {
                    validationResults.Add(new ValidationResult("重複しているタグがあります。", new[] { "TagInlineString" }));
                }
            }

            return validationResults;
        }

        public IEnumerable<Tag> CreateTagCollectionFromInlineText()
        {
            if (string.IsNullOrWhiteSpace(TagInlineString)) return new Tag[0];
            return TagInlineString.Split(' ').Select(CreateTagFromText).Where(tag => tag != null);
        }

        private static Tag CreateTagFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var regex = new Regex(@"\[(.+?)\]");
            if (!regex.IsMatch(text))
            {
                return new Tag(text.Trim(), null);
            }
            var match = regex.Match(text);
            var name = text.Replace(match.Groups[0].Value, "").Trim();
            var version = match.Groups[1].Value.Trim();
            return string.IsNullOrWhiteSpace(name) ? null : new Tag(name, version);
        }
    }

    public enum EntryMode
    {
        SaveDraft,
        PublishItem
    }

    public class DraftPreviewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public bool UnTitled { get; set; }

        public string Html { get; set; }

        public TagCollection Tags { get; set; }

        public DraftPreviewModel()
        {
            Tags = new TagCollection();
        }
    }
}