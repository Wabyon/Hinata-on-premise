using AutoMapper;
using Hinata.Markdown;
using Hinata.Models;

namespace Hinata
{
    public partial class MappingConfig
    {
        private static void CreateItemModelsMap()
        {
            const string askTitlePrefix = @"質問: ";

            Mapper.CreateMap<Item, ItemIndexModel>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name))
                .ForMember(d => d.AuthorDisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                .ForMember(d => d.AuthorIconUrl, o => o.MapFrom(s => s.Author.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
                .ForMember(d => d.Title, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.Title = ((s.Type == ItemType.Ask) ? askTitlePrefix : "") + s.Title;
                });

            Mapper.CreateMap<Item, ItemViewModel>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name))
                .ForMember(d => d.AuthorDisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                .ForMember(d => d.AuthorIconUrl, o => o.MapFrom(s => s.Author.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
                .AfterMap((s, d) =>
                {
                    d.DisplayTitle = ((s.Type == ItemType.Ask) ? askTitlePrefix : "") + s.Title;
                    using (var parser = new MarkdownParser())
                    {
                        d.HtmlBody = parser.Transform(s.Body);
                    }
                });

            Mapper.CreateMap<ItemRevision, ItemRevisionDetailModel>()
                .ForMember(d => d.Comment, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (s.IsFirst && string.IsNullOrWhiteSpace(s.Comment))
                    {
                        d.Comment = "投稿";
                        return;
                    }

                    d.Comment = string.IsNullOrWhiteSpace(s.Comment) ? "(コメントなし)" : s.Comment;
                });
        }
    }
}