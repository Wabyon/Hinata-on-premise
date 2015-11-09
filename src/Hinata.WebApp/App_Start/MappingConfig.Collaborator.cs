using AutoMapper;
using Hinata.Models;

namespace Hinata
{
    public partial class MappingConfig
    {
        private static void CreateCollaboratorModelsMap()
        {
            Mapper.CreateMap<Collaborator, CollaboratorEditModel>()
                .ForMember(d => d.IconUrl, o => o.MapFrom(s => s.IconUrl ?? GlobalSettings.NoImageUserIconUrl));
        }
    }
}