using System;
using System.Collections.Generic;

namespace Hinata.Models
{
    public class ItemIndexModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string AuthorName { get; set; }

        public string AuthorDisplayName { get; set; }

        public string AuthorIconUrl { get; set; }

        public bool IsPublic { get; set; }

        public ItemType Type { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public int CommentCount { get; set; }

        public ItemTagCollection ItemTags { get; set; }

        public bool HasModified { get { return LastModifiedDateTime > CreatedDateTime; } }

        public ItemIndexModel()
        {
            ItemTags = new ItemTagCollection();
        }
    }

    public class ItemViewModel
    {
        private readonly List<CommentViewModel> _comments = new List<CommentViewModel>();

        public string Id { get; set; }

        public string Title { get; set; }

        public string DisplayTitle { get; set; }

        public string AuthorName { get; set; }

        public string AuthorDisplayName { get; set; }

        public string AuthorIconUrl { get; set; }

        public bool IsPublic { get; set; }

        public ItemTagCollection ItemTags { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public bool CanEdit { get; set; }

        public bool HasModified
        {
            get { return LastModifiedDateTime > CreatedDateTime; }
        }

        public string HtmlBody { get; set; }

        public int RevisionCount { get; set; }

        public bool HasRevision { get { return (RevisionCount > 1); } }

        public List<CommentViewModel> Comments
        {
            get { return _comments; }
        }

        public CommentEditModel NewComment { get; set; }

        public ItemViewModel()
        {
            ItemTags = new ItemTagCollection();
        }
    }

    public class ItemRevisionDetailModel
    {
        public string ItemId { get; set; }

        public int RevisionNo { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        public string Comment { get; set; }

        public string Title { get; set; }

        public ItemTagCollection ItemTags { get; set; }

        public string Body { get; set; }

        public bool IsFirst { get; set; }

        public bool IsCurrent { get; set; }

    }
}