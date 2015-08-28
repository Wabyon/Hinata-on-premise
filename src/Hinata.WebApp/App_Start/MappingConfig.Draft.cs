using AutoMapper;
using Hinata.Markdown;
using Hinata.Models;

namespace Hinata
{
    public partial class MappingConfig
    {
        private static void CreateDraftModelsMap()
        {
            const string unTitled = "タイトル未設定";

            Mapper.CreateMap<Draft, DraftIndexModel>()
                .ForMember(d => d.Title, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (string.IsNullOrWhiteSpace(s.Title))
                    {
                        d.Title = unTitled;
                        d.UnTitled = true;
                    }
                    else
                    {
                        d.Title = s.Title;
                        d.UnTitled = false;
                    }
                });
            Mapper.CreateMap<Draft, DraftPreviewModel>()
                .ForMember(d => d.Title, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (string.IsNullOrWhiteSpace(s.Title))
                    {
                        d.Title = unTitled;
                        d.UnTitled = true;
                    }
                    else
                    {
                        d.Title = s.Title;
                        d.UnTitled = false;
                    }
                    using (var parser = new MarkdownParser())
                    {
                        d.Html = parser.Transform(s.Body);
                    }
                });
            Mapper.CreateMap<Draft, DraftEditModel>()
                .ForMember(d => d.ItemType, o => o.MapFrom(s => s.Type))
                .AfterMap((s, d) =>
                {
                    if (s.IsContributed)
                    {
                        d.ItemIsPrivate = !s.ItemIsPublic;
                    }
                    else
                    {
                        d.ItemIsPrivate = false;
                    }

                    using (var parser = new MarkdownParser())
                    {
                        d.Html = parser.Transform(s.Body);
                    }

                    var isFirst = true;
                    foreach (var tag in s.ItemTags)
                    {
                        if (!isFirst) d.TagInlineString += " ";
                        d.TagInlineString += tag.Name;
                        if (tag.Version != null) d.TagInlineString += string.Format("[{0}]", tag.Version);

                        isFirst = false;
                    }
                });
            Mapper.CreateMap<DraftEditModel, Draft>()
                .ForMember(d => d.Type, o => o.MapFrom(s => s.ItemType))
                .ForMember(d => d.ItemIsPublic, o => o.MapFrom(s => !s.ItemIsPrivate))
                .ForMember(d => d.CurrentRevisionNo, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.ItemTags.Clear();
                    foreach (var tag in s.CreateTagCollectionFromInlineText())
                    {
                        d.ItemTags.Add(tag);
                    }
                });
        }
    }
}