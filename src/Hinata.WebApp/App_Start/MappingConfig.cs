namespace Hinata
{
    public partial class MappingConfig
    {
        public static void CreateMap()
        {
            CreateUserModelsMap();
            CreateDraftModelsMap();
            CreateItemModelsMap();
            CreateCommentModelsMap();
        }
    }
}