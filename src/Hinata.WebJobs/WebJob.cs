using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Hinata.Data;
using Hinata.Data.Commands;
using Hinata.Logging;
using Hinata.Markdown;
using Hinata.Search;
using JavaScriptEngineSwitcher.V8;

namespace Hinata
{
    public class WebJob
    {
        private static readonly string ConnectionString;
        private static readonly int Interval;

        private readonly ITraceLogger _logger = LogManager.GetTraceLogger("WEBJOB");
        private Timer _timer;

        static WebJob()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            var intervalSetting = ConfigurationManager.AppSettings["webjob:interval"];
            if (string.IsNullOrWhiteSpace(intervalSetting))
            {
                Interval = 3600000;
            }
            else
            {
                int o;
                Interval = (int.TryParse(intervalSetting, out o) ? o : 3600000);
            }
        }

        public void Start()
        {
            Database.Initialize(ConnectionString);
            DapperConfig.Initialize();
            DefaultTraceLogInitializer.Initialize(ConnectionString, TraceLogLevel.Trace);
            MarkdownParser.RegisterJsEngineType<V8JsEngine>();

            _logger.Trace(string.Format("WEBJOB Start: Interval = {0} ミリ秒", Interval.ToString("##,###")));

            var service = new SearchService(ConnectionString);
            var status = service.GetServiceStatusAsync().Result;

            if (status == ServiceStatus.IndexNotExists)
            {
                service.RecreateEsIndexAsync().Wait();
            }

            _timer = new Timer
            {
                Interval = Interval
            };

            _timer.Elapsed += Execute;
            _timer.Start();
        }

        public void Stop()
        {
            if (_timer == null) return;

            _timer.Dispose();
            _timer = null;

            _logger.Trace("WEBJOB Stop");
        }

        private static void Execute(object sender, EventArgs e)
        {
            var logger = LogManager.GetTraceLogger("WEBJOB");
            logger.Trace(new TraceLogMessage(new {Command = "Execution Start"}, "Elasticsearch"));
            var sw = Stopwatch.StartNew();

            var indexedItemCount = 0;
            try
            {
                var itemDbCommand = new ItemDbCommand(ConnectionString);
                var notIndexedItems = itemDbCommand.GetNotIndexedItemsAsync().Result;
                indexedItemCount = notIndexedItems.Count();

                var searchSearvice = new SearchService(ConnectionString);
                searchSearvice.BulkItemsAsync(notIndexedItems).Wait();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }

            sw.Stop();
            logger.Trace(
                new TraceLogMessage(
                    new { itemDbCommand = "Execution End", IndexedItemCount = indexedItemCount },
                    "Elasticsearch",
                    sw.ElapsedMilliseconds));
        }
    }
}
