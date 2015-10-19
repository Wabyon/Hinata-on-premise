using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Hinata.Data.Commands;
using Hinata.Search.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hinata.Search
{
    public class SearchService : DbCommand
    {
        private readonly SearchServiceConfiguration _config;
        private readonly string _connectionString;

        public SearchService(string connectionString)
            : this(connectionString, SearchServiceConfiguration.Default)
        {
        }

        public SearchService(string connectionString, SearchServiceConfiguration configuration)
            : base(connectionString)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            _config = configuration;
            _connectionString = connectionString;
        }

        public Task RecreateEsIndexAsync()
        {
            return RecreateEsIndexAsync(CancellationToken.None);
        }

        public async Task RecreateEsIndexAsync(CancellationToken cancellationToken)
        {
            #region IndexParameters
            var indexparameters = new
            {
                index = new
                {
                    analysis = new
                    {
                        filter = new
                        {
                            kuromoji_rf = new
                            {
                                type = "kuromoji_readingform",
                                use_romaji = true
                            },
                            kuromoji_pos = new
                            {
                                type = "kuromoji_part_of_speech",
                                enable_position_increment = false,
                                stoptags = new[] {"# verb-main:", "動詞-自立", "助詞-格助詞-一般", "助詞-終助詞"}
                            },
                            kuromoji_ks = new
                            {
                                type = "kuromoji_stemmer",
                                minimum_length = 5
                            },
                            greek_lowercase_filter = new
                            {
                                type = "lowercase",
                                language = "greek"
                            }
                        }
                    },
                    tokenizer = new
                    {
                        kuromoji = new
                        {
                            type = "kuromoji_tokenizer"
                        }
                    },
                    analyzer = new
                    {
                        kuromoji_analyzer = new
                        {
                            type = "custom",
                            tokenizer = "kuromoji_tokenizer",
                            filter =
                                new[]
                                {
                                    "kuromoji_baseform", "kuromoji_ks", "kuromoji_pos", "greek_lowercase_filter",
                                    "cjk_width"
                                }
                        }
                    }
                }
            };
            #endregion

            var node = new Uri(_config.ElasticsearchNode);
            var createContentJson = JsonConvert.SerializeObject(indexparameters);

            using (var createContent = new StringContent(createContentJson))
            using (var client = new HttpClient {BaseAddress = node})
            {
                await client.DeleteAsync(_config.ElasticsearchIndex, cancellationToken).ConfigureAwait(false);

                using (
                    var res =
                        await
                            client.PutAsync(_config.ElasticsearchIndex, createContent, cancellationToken)
                                .ConfigureAwait(false))
                {
                    res.EnsureSuccessStatusCode();
                }
            }

            var itemDbCommand = new ItemDbCommand(_connectionString);
            var items = await itemDbCommand.GetAllAsync(cancellationToken).ConfigureAwait(false);

            await BulkItemsAsync(items, cancellationToken).ConfigureAwait(false);
        }

        public Task IndexItemAsync(Item item)
        {
            return IndexItemAsync(item, CancellationToken.None);
        }

        public async Task IndexItemAsync(Item item, CancellationToken cancellationToken)
        {
            if (item == null) throw new ArgumentNullException("item");

            var node = new Uri(_config.ElasticsearchNode);

            var indexModel = new ItemIndexModel(item);
            using (var content = new StringContent(indexModel.ToJson()))
            using (var client = new HttpClient { BaseAddress = node })
            using (var response = await
                    client.PutAsync(
                        string.Format("/{0}/{1}/{2}", _config.ElasticsearchIndex, item.Type.ToString().ToLower(),
                            item.Id), content, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
            }

            const string sql = @"
INSERT INTO [dbo].[ItemIndexCreatedLogs] (
    [ItemId],
    [IndexCreatedDateTime],
    [RevisionNo]
) VALUES (
    @ItemId,
    @IndexCreatedDateTime,
    @RevisionNo
)
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await
                    cn.ExecuteAsync(sql,
                        new { ItemId = item.Id, IndexCreatedDateTime = DateTime.Now, item.RevisionNo })
                        .ConfigureAwait(false);
            }
        }

        public Task BulkItemsAsync(Item[] items)
        {
            return BulkItemsAsync(items, CancellationToken.None);
        }

        public async Task BulkItemsAsync(Item[] items, CancellationToken cancellationToken)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (!items.Any()) return;

            var node = new Uri(_config.ElasticsearchNode);
            var indexes = items.Select(x => new ItemIndexModel(x)).ToArray();
            var bulk = new StringBuilder();
            foreach (var index in indexes)
            {

                bulk.AppendLine(JsonConvert.SerializeObject(
                    new
                    {
                        index =
                            new
                            {
                                _index = _config.ElasticsearchIndex,
                                _type = index.Type,
                                _id = index.Id
                            }
                    }, Formatting.None));

                bulk.AppendLine(index.ToJson());
            }

            using (var content = new StringContent(bulk.ToString()))
            using (var client = new HttpClient { BaseAddress = node })
            using (var response =
                    await client.PostAsync(string.Format("/{0}/_bulk", _config.ElasticsearchIndex), content, cancellationToken)
                        .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
            }
            const string sql = @"
INSERT INTO [dbo].[ItemIndexCreatedLogs] (
    [ItemId],
    [IndexCreatedDateTime],
    [RevisionNo]
) VALUES (
    @ItemId,
    @IndexCreatedDateTime,
    @RevisionNo
)
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                using (var tr = cn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            await
                                cn.ExecuteAsync(sql,
                                    new {ItemId = item.Id, IndexCreatedDateTime = DateTime.Now, item.RevisionNo}, tr)
                                    .ConfigureAwait(false);

                        }
                        tr.Commit();
                    }
                    catch
                    {
                        tr.Rollback();
                        throw;
                    }
                }
            }
        }

        public Task<string[]> SearchItemIdAsync(SearchCondition condition)
        {
            return SearchItemIdAsync(condition, CancellationToken.None);
        }

        public async Task<string[]> SearchItemIdAsync(SearchCondition condition, CancellationToken cancellationToken)
        {
            var node = new Uri(_config.ElasticsearchNode);
            dynamic query = new ExpandoObject();
            query.fields = new string[]{};
            query.query = new
            {
                simple_query_string = new
                {
                    fields = new[] {"title", "body"},
                    query = string.Join(" ", condition.KeyWords),
                    default_operator = "and"
                }
            };
            if (!condition.IncluidePrivate)
            {
                query.filter = new
                {
                    term = new
                    {
                        isPublic = true
                    }
                };
            }

            using (var content = new StringContent(JsonConvert.SerializeObject(query)))
            using (var client = new HttpClient {BaseAddress = node})
            using (var responce = await client.PostAsync(string.Format("{0}/_search", _config.ElasticsearchIndex), content, cancellationToken).ConfigureAwait(false))
            {
                responce.EnsureSuccessStatusCode();

                var resJson = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                var res = JObject.Parse(resJson);
                var hits = res["hits"]["hits"];
                var ids = hits.Select(x => (string)x["_id"]);

                return ids.ToArray();
            }
        }

        public async Task<ServiceStatus> GetServiceStatusAsync()
        {
            var node = new Uri(_config.ElasticsearchNode);
            using (var client = new HttpClient { BaseAddress = node })
            using (var res = await client.GetAsync(_config.ElasticsearchIndex).ConfigureAwait(false))
            {
                ServiceStatus ss;

                switch (res.StatusCode)
                {
                    case HttpStatusCode.OK:
                        ss = ServiceStatus.IndexExists;
                        break;
                    case HttpStatusCode.NotFound:
                        ss = ServiceStatus.IndexNotExists;
                        break;
                    case HttpStatusCode.InternalServerError:
                        ss = ServiceStatus.NotWork;
                        break;
                    default:
                        ss = ServiceStatus.NotFound;
                        break;
                }

                return ss;
            }
        }
    }
}
