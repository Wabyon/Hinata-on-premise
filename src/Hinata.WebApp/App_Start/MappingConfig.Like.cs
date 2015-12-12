using AutoMapper;
using Hinata.Markdown;
using Hinata.Models;

namespace Hinata
{
    public partial class MappingConfig
    {
        private static void CreateLikeModelsMap()
        {
            Mapper.CreateMap<Like, LikeViewModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ItemId, o => o.MapFrom(s => s.ItemId))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId));
        }
    }
}