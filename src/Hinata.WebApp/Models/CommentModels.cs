using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Hinata.Web.Mvc.DataAnnotations;

namespace Hinata.Models
{
    public class CommentViewModel
    {
        public string CommentId { get; set; }

        public string ItemId { get; set; }

        public string CommentHtmlBody { get; set; }

        public string CommentUserName { get; set; }

        public string CommentUserDisplayName { get; set; }

        public string CommentUserIconUrl { get; set; }

        public DateTime CommentCreatedDateTime { get; internal set; }

        public DateTime CommentLastModifiedDateTime { get; set; }

        public bool IsCommentAuthor { get; set; }
    }

    public class CommentEditModel
    {
        public string CommentId { get; set; }

        public string ItemId { get; set; }

        [PlaceHolder("コメントを入力して下さい。")]
        [AllowHtml]
        [Required]
        public string CommentBody { get; set; }

        public string CommentHtmlBody { get; set; }
    }
}