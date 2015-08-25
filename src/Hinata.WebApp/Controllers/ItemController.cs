using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
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

        [Route("article")]
        [HttpGet]
        public async Task<ActionResult> Articles(int p = 1)
        {
            var skip = MaxItemsOnPage * (p - 1);
            var count = await _itemDbCommand.CountPublicAsync(ItemType.Article);
            var items = await _itemDbCommand.GetPublicAsync(ItemType.Article, skip, MaxItemsOnPage);

            ViewBag.CurrentPage = p;
            ViewBag.HasPreviousPage = (p > 1);
            ViewBag.HasNextPage = (count > MaxItemsOnPage * p);

            ViewBag.Title = "記事";

            return View("Index",Mapper.Map<IEnumerable<ItemIndexModel>>(items));
        }

        [Route("ask")]
        [HttpGet]
        public async Task<ActionResult> Asks(int p = 1)
        {
            var skip = MaxItemsOnPage * (p - 1);
            var count = await _itemDbCommand.CountPublicAsync(ItemType.Ask);
            var items = await _itemDbCommand.GetPublicAsync(ItemType.Ask, skip, MaxItemsOnPage);

            ViewBag.CurrentPage = p;
            ViewBag.HasPreviousPage = (p > 1);
            ViewBag.HasNextPage = (count > MaxItemsOnPage * p);

            ViewBag.Title = "質問";

            return View("Index", Mapper.Map<IEnumerable<ItemIndexModel>>(items));
        }

        [Route("item/{id}")]
        [HttpGet]
        public async Task<ActionResult> Item(string id)
        {
            var item = await _itemDbCommand.FindAsync(id);
            if (item == null) return HttpNotFound();

            var model = Mapper.Map<ItemViewModel>(item);
            model.CanEdit = (item.Author == LogonUser);

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
    }
}