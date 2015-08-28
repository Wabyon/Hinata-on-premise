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

        public int CurrentRevisionNo { get; set; }

        public bool IsNewDraft { get { return (CurrentRevisionNo < 0); } }

        public ItemType ItemType { get; set; }

        [AllowHtml]
        [PlaceHolder("タイトル")]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "本文")]
        public string Body { get; set; }

        [MaxLength(256)]
        [AllowHtml]
        [PlaceHolder("編集履歴コメント（任意）")]
        public string Comment { get; set; }

        [ReadOnly(true)]
        public string Html { get; set; }

        [Display(Name = "タグ")]
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

            if (!string.IsNullOrWhiteSpace(TagInlineString))
            {
                var tags = CreateTagCollectionFromInlineText().ToArray();
                var tagNames = tags.Select(x => x.Name).ToArray();
                var cnt = tagNames.Count();
                var distinctCnt = tagNames.Distinct().Count();
                if (cnt != distinctCnt)
                {
                    validationResults.Add(new ValidationResult("重複しているタグがあります。", new[] { "TagInlineString" }));
                }

                var regex = new Regex(@"^[\<\>\&\""\'\/\*]$", RegexOptions.Compiled);

                if (tagNames.Any(name => regex.IsMatch(name)))
                {
                    validationResults.Add(new ValidationResult("使用できない文字を使ったタグが存在します。", new[] { "TagInlineString" }));
                }

                if (tags.Any(x => x.Name.Length > 32))
                {
                    validationResults.Add(new ValidationResult("一つのタグの長さはは最大32文字までです。", new[] { "TagInlineString" }));
                }

                if (tags.Where(x => x.Version != null).Any(x => x.Version.Length > 16))
                {
                    validationResults.Add(new ValidationResult("一つのタグ・バージョンの長さはは最大16文字までです。", new[] { "TagInlineString" }));
                }
            }

            return validationResults;
        }

        public IEnumerable<ItemTag> CreateTagCollectionFromInlineText()
        {
            if (string.IsNullOrWhiteSpace(TagInlineString)) return new ItemTag[0];
            return TagInlineString.Split(' ').Select(CreateTagFromText).Where(tag => tag != null);
        }

        private static ItemTag CreateTagFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var regex = new Regex(@"\[(.+?)\]");
            if (!regex.IsMatch(text))
            {
                return new ItemTag(text.Trim(), null);
            }
            var match = regex.Match(text);
            var name = text.Replace(match.Groups[0].Value, "").Trim();
            var version = match.Groups[1].Value.Trim();
            return string.IsNullOrWhiteSpace(name) ? null : new ItemTag(name, version);
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

        public ItemTagCollection ItemTags { get; set; }

        public DraftPreviewModel()
        {
            ItemTags = new ItemTagCollection();
        }
    }
}