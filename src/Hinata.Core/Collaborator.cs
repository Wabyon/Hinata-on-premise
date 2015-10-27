namespace Hinata
{
    public class Collaborator : User
    {
        public RoleType Role { get; set; }

        public Collaborator()
        {
        }

        public Collaborator(User user)
        {
            Id = user.Id;
            LogonName = user.LogonName;
            Name = user.Name;
            DisplayName = user.DisplayName;
            IconUrl = user.IconUrl;
        }
    }
}
