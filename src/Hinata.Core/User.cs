using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hinata
{
    public class User : IEquatable<User>
    {
        public string Id { get; set; }

        public string LogonName { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string IconUrl { get; set; }

        public bool IsRegistered
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public User()
        {
        }

        public User(string logonName)
        {
            Id = CreateId(logonName);
            LogonName = logonName;
        }

        private static string CreateId(string logonName)
        {
            using (var md5 = MD5.Create())
            {
                return
                string.Concat(
                    md5.ComputeHash(Encoding.UTF8.GetBytes(logonName))
                        .Select(x => string.Format("{0:x2}", x)));
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(User left, User right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(User left, User right)
        {
            return !Equals(left, right);
        }

        private static readonly string[] ReservedNames = {"admin", "admins", "administrator", "administrators", "hinata", "unknown"};

        public static bool IsReservedName(string name)
        {
            return ReservedNames.Contains(name.ToLower());
        }

        public static readonly User Unknown = new User
        {
            Id = "00000000000000000000000000000000",
            Name = "unknown",
            DisplayName = "unknown",
            LogonName = "unknown"
        };
    }
}
