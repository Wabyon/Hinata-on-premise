using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Hinata.Markdown
{
    internal static class HtmlUtility
    {
        private static readonly IDictionary<string, string[]> ValidHtmlTags = new Dictionary<string, string[]>
        {
            {"p", new[] {"style", "class", "align"}},
            {"div", new[] {"style", "class", "align"}},
            {"span", new[] {"style", "class"}},
            {"br", new[] {"style", "class"}},
            {"hr", new[] {"style", "class"}},
            {"label", new[] {"style", "class"}},
            {"h1", new[] {"style", "class"}},
            {"h2", new[] {"style", "class"}},
            {"h3", new[] {"style", "class"}},
            {"h4", new[] {"style", "class"}},
            {"h5", new[] {"style", "class"}},
            {"h6", new[] {"style", "class"}},
            {"font", new[] {"style", "class", "color", "face", "size"}},
            {"strong", new[] {"style", "class"}},
            {"b", new[] {"style", "class"}},
            {"em", new[] {"style", "class"}},
            {"i", new[] {"style", "class"}},
            {"u", new[] {"style", "class"}},
            {"strike", new[] {"style", "class"}},
            {"ol", new[] {"style", "class"}},
            {"ul", new[] {"style", "class"}},
            {"li", new[] {"style", "class"}},
            {"blockquote", new[] {"style", "class"}},
            {"pre", new[] {"style", "class"}},
            {"code", new[] {"style", "class"}},
            {"a", new[] {"style", "class", "href", "title"}},
            {
                "img", new[]
                {
                    "style", "class", "src", "height", "width",
                    "alt", "title", "hspace", "vspace", "border"
                }
            },
            {"table", new[] {"style", "class"}},
            {"thead", new[] {"style", "class"}},
            {"tbody", new[] {"style", "class"}},
            {"tfoot", new[] {"style", "class"}},
            {"th", new[] {"style", "class", "scope"}},
            {"tr", new[] {"style", "class"}},
            {"td", new[] {"style", "class", "colspan"}},
            {"q", new[] {"style", "class", "cite"}},
            {"cite", new[] {"style", "class"}},
            {"abbr", new[] {"style", "class"}},
            {"acronym", new[] {"style", "class"}},
            {"del", new[] {"style", "class"}},
            {"ins", new[] {"style", "class"}},
            {"dl", new[] {"style", "class"}},
            {"dt", new[] {"style", "class"}},
            {"dd", new[] {"style", "class"}},
            {"dfn", new[] {"style", "class"}},
        };

        private static readonly string[] NotUrlEncodeAttributes = { "style", "class" };

        public static string SanitizeHtml(string source)
        {
            var html = GetHtml(source);

            if (html == null) return "";

            var allNodes = html.DocumentNode;

            var whitelist = (from kv in ValidHtmlTags select kv.Key).ToArray();

            CleanNodes(allNodes, whitelist);

            foreach (var tag in ValidHtmlTags)
            {
                var tag1 = tag;
                var nodes = allNodes.DescendantsAndSelf().Where(n => n.Name == tag1.Key);

                foreach (var a in from n in nodes where n.HasAttributes select n.Attributes.ToArray() into attr from a in attr select a)
                {
                    if (!tag.Value.Contains(a.Name))
                    {
                        a.Remove();
                    }
                    else if (!NotUrlEncodeAttributes.Contains(a.Name))
                    {
                        a.Value = Microsoft.Security.Application.Encoder.UrlPathEncode(a.Value);
                    }
                }
            }

            return allNodes.InnerHtml;
        }

        public static string StripHtml(string source)
        {
            source = SanitizeHtml(source);

            if (string.IsNullOrEmpty(source)) return "";

            var html = GetHtml(source);
            var result = new StringBuilder();

            foreach (var node in html.DocumentNode.ChildNodes)
            {
                result.Append(node.InnerText);
            }

            return result.ToString();
        }

        private static void CleanNodes(HtmlNode node, string[] whitelist)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                if (!whitelist.Contains(node.Name))
                {
                    node.ParentNode.RemoveChild(node);
                    return;
                }
            }

            if (node.HasChildNodes) CleanChildren(node, whitelist);
        }

        private static void CleanChildren(HtmlNode parent, string[] whitelist)
        {
            for (var i = parent.ChildNodes.Count - 1; i >= 0; i--)
            {
                CleanNodes(parent.ChildNodes[i], whitelist);
            }
        }

        private static HtmlDocument GetHtml(string source)
        {
            var html = new HtmlDocument
            {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };

            html.LoadHtml(source);

            return html;
        }
    }
}
