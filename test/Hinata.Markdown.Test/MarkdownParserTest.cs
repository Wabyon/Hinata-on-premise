using System.Net;
using JavaScriptEngineSwitcher.V8;
using NUnit.Framework;

namespace Hinata.Markdown
{
    [TestFixture]
    public class MarkdownParserTest
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            MarkdownParser.RegisterJsEngineType<V8JsEngine>();
        }

        [TestCase("#H1", "<h1>H1</h1>\n")]
        [TestCase("# H1", "<h1>H1</h1>\n")]
        [TestCase("H1\n==", "<h1>H1</h1>\n")]
        [TestCase("H1\n======", "<h1>H1</h1>\n")]
        [TestCase("##H2", "<h2>H2</h2>\n")]
        [TestCase("## H2", "<h2>H2</h2>\n")]
        [TestCase("H2\n--", "<h2>H2</h2>\n")]
        [TestCase("H2\n------", "<h2>H2</h2>\n")]
        [TestCase("###H3", "<h3>H3</h3>\n")]
        [TestCase("### H3", "<h3>H3</h3>\n")]
        [TestCase("####H4", "<h4>H4</h4>\n")]
        [TestCase("#### H4", "<h4>H4</h4>\n")]
        [TestCase("#####H5", "<h5>H5</h5>\n")]
        [TestCase("##### H5", "<h5>H5</h5>\n")]
        public void HeadingTest(string markdown, string expected)
        {
            using (var parser = new MarkdownParser())
            {
                var html = parser.Transform(markdown);
                Assert.AreEqual(expected, html);
            }
        }

        [Test]
        public void Test()
        {
            using (var parser = new MarkdownParser())
            {
                var html = parser.Transform("H1\n==");
            }            
        }

        [TestCase("#H1", "H1\n")]
        [TestCase("line", "line\n")]
        [TestCase(@"<a href=""http://lonk"">tag</a>", "tag\n")]

        public void StripTest(string markdown, string expected)
        {
            using (var parser = new MarkdownParser())
            {
                var stripText = parser.Strip(markdown);
                Assert.AreEqual(expected, stripText);
            }
        }
    }
}
