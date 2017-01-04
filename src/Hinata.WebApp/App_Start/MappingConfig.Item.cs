using System.Collections.Generic;
using AutoMapper;
using Hinata.Markdown;
using Hinata.Models;

namespace Hinata
{
    public partial class MappingConfig
    {
        private static void CreateItemModelsMap()
        {
            Mapper.CreateMap<Item, ItemIndexModel>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name))
                .ForMember(d => d.AuthorDisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                .ForMember(d => d.AuthorIconUrl, o => o.MapFrom(s => s.Author.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
                .ForMember(d => d.EditorName, o => o.MapFrom(s => s.Editor.Name))
                .ForMember(d => d.EditorDisplayName, o => o.MapFrom(s => s.Editor.DisplayName))
                .ForMember(d => d.EditorIconUrl, o => o.MapFrom(s => s.Editor.IconUrl ?? GlobalSettings.NoImageUserIconUrl));

            Mapper.CreateMap<Item, ItemViewModel>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name))
                .ForMember(d => d.AuthorDisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                .ForMember(d => d.AuthorIconUrl, o => o.MapFrom(s => s.Author.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
                .ForMember(d => d.EditorName, o => o.MapFrom(s => s.Editor.Name))
                .ForMember(d => d.EditorDisplayName, o => o.MapFrom(s => s.Editor.DisplayName))
                .ForMember(d => d.EditorIconUrl, o => o.MapFrom(s => s.Editor.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
                .AfterMap((s, d) =>
                {
                    using (var parser = new MarkdownParser())
                    {
                        d.HtmlBody = parser.Transform(s.Body);
                    }
                });

            Mapper.CreateMap<ItemRevision, ItemRevisionDetailModel>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name))
                .ForMember(d => d.AuthorDisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                .ForMember(d => d.AuthorIconUrl, o => o.MapFrom(s => s.Author.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
                .ForMember(d => d.EditorName, o => o.MapFrom(s => s.Editor.Name))
                .ForMember(d => d.EditorDisplayName, o => o.MapFrom(s => s.Editor.DisplayName))
                .ForMember(d => d.EditorIconUrl, o => o.MapFrom(s => s.Editor.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
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

            Mapper.CreateMap<Item, ItemEditCollaboratorsModel>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name))
                .ForMember(d => d.AuthorDisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                .ForMember(d => d.AuthorIconUrl, o => o.MapFrom(s => s.Author.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
                .ForMember(d => d.Collaborators, o => o.MapFrom(s => Mapper.Map<IEnumerable<CollaboratorEditModel>>(s.Collaborators)));
        }
    }
}