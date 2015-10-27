namespace Hinata.Models
{
    public class CollaboratorSearchResultModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string IconUrl { get; set; }
    }

    public class CollaboratorEditModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string IconUrl { get; set; }

        public RoleType Role { get; set; }
    }
}