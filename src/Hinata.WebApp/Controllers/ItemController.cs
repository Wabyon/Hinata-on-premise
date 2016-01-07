using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Hinata.Data.Commands;
using Hinata.Models;
using Hinata.Web.Mvc;

namespace Hinata.Controllers
{
    public class ItemController : WindowsAuthenticationContoller
    {
        private readonly ItemDbCommand _itemDbCommand = new ItemDbCommand(GlobalSettings.DefaultConnectionString);
        private readonly CommentDbCommand _commentDbCommand = new CommentDbCommand(GlobalSettings.DefaultConnectionString);
        private readonly UserDbCommand _userDbCommand = new UserDbCommand(GlobalSettings.DefaultConnectionString);
        private const int MaxItemsOnPage = 15;

        [Route]
        [Route("item")]
        [HttpGet]
        public async Task<ActionResult> Index(int p = 1)
        {
            var skip = MaxItemsOnPage*(p - 1);
            var count = await _itemDbCommand.CountPublicAsync();
            var items = await _itemDbCommand.GetPublicAsync(skip, MaxItemsOnPage);

            ViewBag.CurrentPage = p;
            ViewBag.HasPreviousPage = (p > 1);
            ViewBag.HasNextPage = (count > MaxItemsOnPage*p);

            return View(Mapper.Map<IEnumerable<ItemIndexModel>>(items));
        }

        [Route("newest")]
        [HttpGet]
        public async Task<ActionResult> Newest(int p = 1)
        {
            var skip = MaxItemsOnPage * (p - 1);
            var count = await _itemDbCommand.CountPublicAsync();
            var items = await _itemDbCommand.GetPublicNewerAsync(skip, MaxItemsOnPage);

            ViewBag.CurrentPage = p;
            ViewBag.HasPreviousPage = (p > 1);
            ViewBag.HasNextPage = (count > MaxItemsOnPage * p);

            ViewBag.Title = "新着";

            return View("Index", Mapper.Map<IEnumerable<ItemIndexModel>>(items));
        }

        [Route("item/{id}")]
        [HttpGet]
        public async Task<ActionResult> Item(string id)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();

            var model = Mapper.Map<ItemViewModel>(item);
            model.CanEdit = LogonUser.IsEntitledToEditItem(item);
            model.CanDelete = LogonUser.IsEntitledToDeleteItem(item);
            model.CanEditCollarborators = LogonUser.IsEntitledToEditItemCollaborators(item);
            model.CanWriteComments = LogonUser.IsEntitledToWriteComments(item);

            ViewBag.Title = model.DisplayTitle;

            var comments = await _commentDbCommand.GetByItemAsync(item);
            foreach (var comment in comments)
            {
                var commentModel = Mapper.Map<CommentViewModel>(comment);
                if (comment.User == LogonUser)
                {
                    commentModel.IsCommentAuthor = true;
                }
                model.Comments.Add(commentModel);
            }

            model.NewComment = (LogonUser == null) ? new CommentEditModel() : Mapper.Map<CommentEditModel>(item.NewComment(LogonUser));

            return View("Item", model);
        }

        [Route("item/{id}/raw/{title}.md")]
        [HttpGet]
        public async Task<ActionResult> Raw(string id)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();

            var result = new ContentResult
            {
                Content = item.Body,
                ContentType = "text/plane",
                ContentEncoding = Encoding.UTF8
            };

            return result;
        }

        [ValidateAntiForgeryToken]
        [Route("item/{id}/delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();

            if (item.Author != LogonUser) return new HttpUnauthorizedResult();

            await _itemDbCommand.DeleteAsync(item.Id);

            return RedirectToAction("Index", "Item");
        }

        [ValidateAntiForgeryToken]
        [Route("item/{id}/savecomment")]
        [HttpPost]
        public async Task<ActionResult> SaveComment(CommentEditModel model)
        {
            if (!ModelState.IsValid) return PartialView("_CommentEdit", model);

            var item = await _itemDbCommand.FindAsync(model.ItemId);
            if (item == null) return HttpNotFound();

            var comment = await _commentDbCommand.FindAsync(model.CommentId);
            if (comment == null)
            {
                comment = item.NewComment(LogonUser);
                Mapper.Map(model, comment);
            }
            else
            {
                comment.LastModifiedDateTime = DateTime.Now;
            }

            await _commentDbCommand.SaveAsync(comment);

            return RedirectToAction("Item", new {id = model.ItemId});
        }

        [Route("item/{id}/revisions")]
        [HttpGet]
        public async Task<ActionResult> Revisions(string id)
        {
            var itemRevisions = await _itemDbCommand.GetRevisionsAsync(id);

            if (!itemRevisions.Any()) return HttpNotFound();

            var model = Mapper.Map<IEnumerable<ItemRevisionDetailModel>>(itemRevisions);

            return View(model);
        }

        [Route("item/{id}/revisions/{revisionNo}")]
        [HttpGet]
        public async Task<ActionResult> Revision(string id, int revisionNo)
        {
            var itemRevision = await _itemDbCommand.FindRevisionAsync(id, revisionNo);

            if (itemRevision == null) return HttpNotFound();

            var model = Mapper.Map<ItemRevisionDetailModel>(itemRevision);

            return PartialView("_RevisionDetail", model);
        }

        [Route("item/{id}/collaborators/edit")]
        [HttpGet]
        public async Task<ActionResult> EditCollaborators(string id)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();
            if (!LogonUser.IsEntitledToEditItemCollaborators(item)) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var model = Mapper.Map<ItemEditCollaboratorsModel>(item);

            return View(model);
        }

        [Route("item/{id}/collaborators/add")]
        [HttpPost]
        public async Task<ActionResult> AddCollaborator(string id, string userId)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();
            if (!LogonUser.IsEntitledToEditItemCollaborators(item)) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var targetUser = await _userDbCommand.FindAsync(userId);
            if (targetUser == null) return HttpNotFound();

            if (item.Collaborators.Contains(targetUser)) throw new InvalidOperationException();

            var newCollaborator = new Collaborator(targetUser) { Role = RoleType.Member };

            await _itemDbCommand.AddCollaboratorAsync(item, newCollaborator);

            var model = Mapper.Map<CollaboratorEditModel>(newCollaborator);

            return PartialView("_CollaboratorEdit", model);
        }

        [Route("item/{id}/collaborators/remove")]
        [HttpPost]
        public async Task<ActionResult> RemoveCollaborator(string id, string userId)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();
            if (!LogonUser.IsEntitledToEditItemCollaborators(item)) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var deleteCollaborator = item.Collaborators.FirstOrDefault(x => x.Id == userId);

            if (deleteCollaborator == null) throw new InvalidOperationException();

            await _itemDbCommand.RemoveCollaboratorAsync(item, deleteCollaborator);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [Route("item/{id}/collaborators/add/suggestions")]
        [HttpPost]
        public async Task<ActionResult> SearchCollaborator(string id, string query)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();

            var searchText = query.Replace("　", "").Split(' ');

            var users = (await _userDbCommand.SearchAsync(searchText)).ToList();

            users.Remove(item.Author);
            foreach (var collaborator in item.Collaborators)
            {
                users.Remove(collaborator);
            }

            var model = Mapper.Map<IEnumerable<CollaboratorSearchResultModel>>(users);

            return PartialView("_CollaboratorSearchResult", model);
        }


        [Route("item/{id}/collaborators/update/role")]
        [HttpPost]
        public async Task<ActionResult> UpdateCollaboratorRole(string id, string userId, RoleType type)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();
            if (!LogonUser.IsEntitledToEditItemCollaborators(item)) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var targetCollaborator = item.Collaborators.FirstOrDefault(x => x.Id == userId);
            if (targetCollaborator == null) throw new InvalidOperationException();

            targetCollaborator.Role = type;

            await _itemDbCommand.SaveCollaboratorsAsync(item);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}