using AutoMapper;
using Hinata.Markdown;
using Hinata.Models;

namespace Hinata
{
    public partial class MappingConfig
    {
        private static void CreateCommentModelsMap()
        {
            Mapper.CreateMap<Comment, CommentViewModel>()
                .ForMember(d => d.CommentId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CommentUserName, o => o.MapFrom(s => s.User.Name))
                .ForMember(d => d.CommentUserDisplayName, o => o.MapFrom(s => s.User.DisplayName))
                .ForMember(d => d.CommentUserIconUrl, o => o.MapFrom(s => s.User.IconUrl ?? GlobalSettings.NoImageUserIconUrl))
                .ForMember(d => d.CommentCreatedDateTime, o => o.MapFrom(s => s.CreatedDateTime))
                .ForMember(d => d.CommentLastModifiedDateTime, o => o.MapFrom(s => s.LastModifiedDateTime))
                .AfterMap((s, d) =>
                {
                    using (var parser = new MarkdownParser())
                    {
                        d.CommentHtmlBody = parser.Transform(s.Body);
                    }
                });

            Mapper.CreateMap<Comment, CommentEditModel>()
                .ForMember(d => d.CommentId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CommentBody, o => o.MapFrom(s => s.Body))
                .AfterMap((s, d) =>
                {
                    using (var parser = new MarkdownParser())
                    {
                        d.CommentHtmlBody = parser.Transform(s.Body);
                    }
                });

            Mapper.CreateMap<CommentEditModel, Comment>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.CommentId))
                .ForMember(d => d.Body, o => o.MapFrom(s => s.CommentBody));
        }
    }
}