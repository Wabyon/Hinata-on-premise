using System;
using System.Collections.Generic;
using System.Linq;
using Hinata.Collections;
using Hinata.Exceptions;

namespace Hinata
{
    public class Item
    {
        private readonly ItemTagCollection _itemTags = new ItemTagCollection();
        private readonly CollaboratorCollection _collaborators = new CollaboratorCollection();

        public string Id { get; internal set; }

        public bool IsPublic { get; internal set; }

        public ItemType Type { get; internal set; }

        public User Author { get; internal set; }

        /// <summary>編集者</summary>
        public User Editor { get; internal set; }

        public string Title { get; internal set; }

        public string Body { get; internal set; }

        public int LikeCount { get; internal set; }

        public int CommentCount { get; internal set; }

        public ItemTagCollection ItemTags { get { return _itemTags; } }

        public DateTime CreatedDateTime { get; internal set; }

        public DateTime LastModifiedDateTime { get; set; }

        public int RevisionNo { get; internal set; }

        public int RevisionCount { get; internal set; }

        internal string Comment { get; set; }

        public DateTime? PublishSince { get; set; }

        public DateTime? PublishUntil { get; set; }

        /// <summary>共同編集者</summary>
        public IReadOnlyCollection<Collaborator> Collaborators
        {
            get { return _collaborators; }
        }

        internal Item()
        {
        }

        public Draft ToDraft(User editor)
        {
            if (!editor.IsEntitledToEditItem(this)) throw new NotEntitledToEditItemException();

            return new Draft(this)
            {
                CurrentRevisionNo = RevisionNo,
                Editor = editor,
                PublishedBody = Body,
            };
        }

        public Like NewLike(User user)
        {
            if (user == null) throw new ArgumentNullException("user");

            LikeCount++;

            return new Like(this)
            {
                UserId = user.Id,
            };
        }

        public Comment NewComment(User user)
        {
            if (user == null) throw new ArgumentNullException("user");

            CommentCount++;

            return new Comment(this)
            {
                User = user,
                CreatedDateTime = DateTime.Now,
                LastModifiedDateTime = DateTime.Now
            };
        }

        /// <summary>記事に共同編集者を追加します</summary>
        /// <param name="collaborator"></param>
        internal void AddCollaborator(Collaborator collaborator)
        {
            if (collaborator == null) throw new ArgumentNullException("collaborator");
            if (_collaborators.Contains(collaborator)) throw new InvalidOperationException("target user is already included in collaborators.");

            _collaborators.Add(collaborator);
        }

        internal void RemoveCollaborator(Collaborator collaborator)
        {
            if (collaborator == null) throw new ArgumentNullException("collaborator");
            if (!_collaborators.Contains(collaborator)) throw new InvalidOperationException("target user is not included in collaborators.");

            _collaborators.Remove(collaborator);
        }

        /// <summary>記事の共同編集者を全てクリアします</summary>
        internal void ClearAllCollaborators()
        {
            _collaborators.Clear();
        }
    }
}
