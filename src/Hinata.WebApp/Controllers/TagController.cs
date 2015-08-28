using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Hinata.Data.Commands;
using Hinata.Models;
using Hinata.Web.Mvc;

namespace Hinata.Controllers
{
    public class TagController : WindowsAuthenticationContoller
    {
        private readonly TagDbCommand _tagDbCommand = new TagDbCommand(GlobalSettings.DefaultConnectionString);
        private readonly ItemDbCommand _itemDbCommand = new ItemDbCommand(GlobalSettings.DefaultConnectionString);
        private const int MaxItemsOnPage = 15;

        [Route("tag")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var tags = await _tagDbCommand.GetAllAsync();

            return View(tags.OrderByDescending(x => x.AllItemCount));
        }

        [Route("tag/{name}")]
        [HttpGet]
        public async Task<ActionResult> ItemList(string name, int p = 1)
        {
            var tag = await _tagDbCommand.FindAsync(name);
            if (tag == null) return HttpNotFound();

            var skip = MaxItemsOnPage*(p - 1);
            var count = await _itemDbCommand.CountPublicByTagAsync(tag);
            var items = await _itemDbCommand.GetPublicByTagAsync(tag, skip, MaxItemsOnPage);

            ViewBag.CurrentPage = p;
            ViewBag.HasPreviousPage = (p > 1);
            ViewBag.HasNextPage = (count > MaxItemsOnPage * p);

            ViewBag.TagName = tag.Name;

            return View(Mapper.Map<IEnumerable<ItemIndexModel>>(items));
        }
    }
}