using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Hinata.Data.Commands;
using Hinata.Logging;
using Hinata.Models;
using Hinata.Search;
using Hinata.Web.Mvc;

namespace Hinata.Controllers
{
    public class SearchController : WindowsAuthenticationContoller
    {
        private readonly SearchService _searchService = new SearchService(GlobalSettings.DefaultConnectionString);
        private readonly ItemDbCommand _itemDbCommand = new ItemDbCommand(GlobalSettings.DefaultConnectionString);
        private readonly ITraceLogger _logger = LogManager.GetTraceLogger("Search");
        private const int MaxItemsOnPage = 15;

        [Route("search")]
        [HttpGet]
        [ValidateInput(false)]
        public async Task<ActionResult> Index(string q = null, int p = 1)
        {
            ViewBag.Query = q;
            ViewBag.Title = string.Format("「{0}」の検索結果", q);

            var model = new SearchResultModel {Query = q};

            var condition = new SearchCondition(q);

            try
            {
                var skip = MaxItemsOnPage * (p - 1);
                var ids = await _searchService.SearchItemIdAsync(condition);
                var count = ids.Count();
                
                ViewBag.CurrentPage = p;
                ViewBag.HasPreviousPage = (p > 1);
                ViewBag.HasNextPage = (count > MaxItemsOnPage * p);

                var items = await _itemDbCommand.GetByIdsAsync(ids, skip, MaxItemsOnPage);
                model.Items = Mapper.Map<IEnumerable<ItemIndexModel>>(items);
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                model.HasSearchServiceError = true;
            }

            return View(model);
        }
    }
}