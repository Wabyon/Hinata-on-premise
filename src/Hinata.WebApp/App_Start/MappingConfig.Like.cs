using AutoMapper;
using Hinata.Models;

namespace Hinata
{
    public partial class MappingConfig
    {
        private static void CreateLikeModelsMap()
        {
            Mapper.CreateMap<Like, LikeViewModel>();
        }
    }
}