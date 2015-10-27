using AutoMapper;
using Hinata.Models;

namespace Hinata
{
    public partial class MappingConfig
    {
        private static void CreateUserModelsMap()
        {
            Mapper.CreateMap<User, UserUpdateModel>();

            Mapper.CreateMap<UserCreateModel, User>();
            Mapper.CreateMap<UserUpdateModel, User>();

            Mapper.CreateMap<User, CollaboratorSearchResultModel>()
                .ForMember(d => d.IconUrl, o => o.MapFrom(s => s.IconUrl ?? GlobalSettings.NoImageUserIconUrl));
        }
    }
}