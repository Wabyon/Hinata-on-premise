using System.Configuration;

namespace Hinata.Search
{
    public class SearchServiceConfiguration
    {
        public string ElasticsearchNode { get; set; }

        public string ElasticsearchIndex { get; set; }

        static SearchServiceConfiguration()
        {
            Default = new SearchServiceConfiguration
            {
                ElasticsearchNode = ConfigurationManager.AppSettings["es:node"] ?? @"http://localhost:9200",
                ElasticsearchIndex = ConfigurationManager.AppSettings["es:index"] ?? @"hinata"
            };
        }
        public static SearchServiceConfiguration Default;
    }
}