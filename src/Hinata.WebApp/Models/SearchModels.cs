using System.Collections.Generic;
using AutoMapper;

namespace Hinata.Models
{
    public class SearchResultModel
    {
        public string Query { get; set; }

        public bool HasSearchServiceError { get; set; }

        public IEnumerable<ItemIndexModel> Items { get; set; }

        public SearchResultModel()
        {
            Items = new List<ItemIndexModel>();
        }

        public SearchResultModel(IEnumerable<Item> items)
        {
            Items = Mapper.Map<IEnumerable<ItemIndexModel>>(items);
        }
    }
}