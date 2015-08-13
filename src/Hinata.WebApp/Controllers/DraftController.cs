using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Hinata.Data.Commands;
using Hinata.Markdown;
using Hinata.Models;
using Hinata.Web.Mvc;

namespace Hinata.Controllers
{
    public class DraftController : WindowsAuthenticationContoller
    {
        private readonly DraftDbCommand _draftDbCommand = new DraftDbCommand(GlobalSettings.DefaultConnectionString);
        private readonly ItemDbCommand _itemDbCommand = new ItemDbCommand(GlobalSettings.DefaultConnectionString);

        [Route("draft")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var drafts = await _draftDbCommand.GetByAuthorAsync(LogonUser);
            var model = Mapper.Map<IEnumerable<DraftIndexModel>>(drafts);
            return View(model);
        }

        [Route("draft/new")]
        [HttpGet]
        public ActionResult New(ItemType itemType = ItemType.Article)
        {
            var draft = Draft.NewDraft(LogonUser, itemType);
            var model = Mapper.Map<DraftEditModel>(draft);

            ViewBag.Title = "新規作成";

            return View("Edit", model);
        }

        [Route("draft/{id}/edit")]
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            var draft = await _draftDbCommand.FindAsync(id);
            if (draft == null)
            {
                var item = await _itemDbCommand.FindAsync(id);
                if (item == null) return HttpNotFound();
                draft = item.ToDraft();
                await _draftDbCommand.SaveAsync(draft);
            }

            if (string.IsNullOrWhiteSpace(draft.Title))
            {
                ViewBag.Title = "編集";
            }
            else
            {
                ViewBag.Title = "編集: " + draft.Title;
            }

            var model = Mapper.Map<DraftEditModel>(draft);
            return View(model);
        }

        [Route("draft/save")]
        [HttpPost]
        public async Task<ActionResult> Save(DraftEditModel model)
        {
            model.EntryMode = EntryMode.SaveDraft;
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                using (var parser = new MarkdownParser())
                {
                    model.Html = parser.Transform(model.Body);
                    return View("Edit", model);
                }
            }

            var draft = await _draftDbCommand.FindAsync(model.Id) ?? Draft.NewDraft(LogonUser, model.ItemType);
            Mapper.Map(model, draft);

            draft.LastModifiedDateTime = DateTime.Now;
            await _draftDbCommand.SaveAsync(draft);
            return RedirectToAction("Index", "Draft");
        }

        [Route("draft/publish")]
        [HttpPost]
        public async Task<ActionResult> Publish(DraftEditModel model)
        {
            model.EntryMode = EntryMode.PublishItem;
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                using (var parser = new MarkdownParser())
                {
                    model.Html = parser.Transform(model.Body);
                    return View("Edit", model);
                }
            }

            var draft = await _draftDbCommand.FindAsync(model.Id) ?? Draft.NewDraft(LogonUser, model.ItemType);
            Mapper.Map(model, draft);
            var item = draft.ToItem();

            await _itemDbCommand.SaveAsync(item);
            await _draftDbCommand.DeleteAsync(draft.Id);

            return RedirectToAction("Index", "Item");
        }

        [Route("draft/autosave")]
        [HttpPost]
        public async Task<JsonResult> AutoSave(DraftEditModel model)
        {
            var draft = Mapper.Map<Draft>(model);
            draft.Author = LogonUser;
            draft.LastModifiedDateTime = DateTime.Now;
            await _draftDbCommand.SaveAsync(draft);

            return Json(new { draft.Id, Url = Url.Action("Edit", "Draft", new { id = draft.Id }) });
        }

        [Route("draft/html")]
        [HttpPost]
        public ContentResult ConvertToHtml(string markdown)
        {
            using (var parser = new MarkdownParser())
            {
                return new ContentResult
                {
                    Content = parser.Transform(markdown),
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "text/html"
                };
            }
        }

        [Route("draft/{id}/preview")]
        [HttpGet]
        public async Task<ActionResult> Preview(string id)
        {
            var draft = await _draftDbCommand.FindAsync(id);
            if (draft == null) return HttpNotFound();

            var model = Mapper.Map<DraftPreviewModel>(draft);

            return PartialView("_Preview", model);
        }

        [HttpGet]
        [Route("draft/{id}/delete")]
        public async Task<ActionResult> Delete(string id)
        {
            var draft = await _draftDbCommand.FindAsync(id);
            if (draft == null || draft.Author != LogonUser) return HttpNotFound();

            await _draftDbCommand.DeleteAsync(id);

            return RedirectToAction("Index");
        }
    }
}