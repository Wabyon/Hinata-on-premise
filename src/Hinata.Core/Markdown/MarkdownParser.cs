using System;
using JavaScriptEngineSwitcher.Core;

namespace Hinata.Markdown
{
    public class MarkdownParser : IDisposable
    {
        private static Type _jsEngineType;
        private readonly object _compilationSynchronizer = new object();
        private IJsEngine _jsEngine;
        private bool _initialized;
        private bool _disposed;

        public static void RegisterJsEngineType<T>()
            where T : IJsEngine, new()
        {
            _jsEngineType = typeof(T);
        }

        public MarkdownParser()
        {
            if (_jsEngineType == null) throw new InvalidOperationException("JsEngineType is not registered.");

            _jsEngine = Activator.CreateInstance(_jsEngineType) as IJsEngine;
            if (_jsEngine == null) throw new InvalidOperationException("JsEngine was not activated.");
        }

        private void Initialize()
        {
            if (_initialized) return;

            var type = GetType();

            _jsEngine.Execute("this.window = this;");
            _jsEngine.ExecuteResource("Hinata.Scripts.marked.marked.js", type);
            _jsEngine.ExecuteResource("Hinata.Scripts.highlightjs.highlight.pack.js", type);

            _initialized = true;
        }

        public string Transform(string text, bool sanitize = true)
        {
            if (text == null) return "";

            string result;

            lock (_compilationSynchronizer)
            {
                Initialize();

                _jsEngine.Evaluate(@"
marked.setOptions({
    gfm: true,
    tables: true,
    breaks: true,
    pedantic: false,
    sanitize: false,
    smartLists: true,
    silent: false,
    highlight: function (code) {
        return hljs.highlightAuto(code).value;
    },
    langPrefix: '',
    smartypants: false,
    headerPrefix: '',
    renderer: new marked.Renderer(),
    xhtml: false
});");

                result = _jsEngine.CallFunction<string>("marked", text);
            }

            return (sanitize) ? HtmlUtility.SanitizeHtml(result) : result;
        }

        public string Strip(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var html = Transform(text);

            return HtmlUtility.StripHtml(html);
        }


        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_jsEngine == null) return;
            _jsEngine.Dispose();

            _jsEngine = null;
        }
    }
}
