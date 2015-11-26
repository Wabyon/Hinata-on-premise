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

        /// <summary>記事の著者などが不明な場合の代替ユーザー</summary>
        public static readonly User Unknown = new User
        {
            Id = "00000000000000000000000000000000",
            Name = "unknown",
            DisplayName = "unknown",
            LogonName = "unknown"
        };

        /// <summary>ユーザー登録していないアクセスユーザー</summary>
        public static readonly User Anonymous = new User
        {
            Id = "00000000000000000000000000000001",
            Name = "anonymous",
            DisplayName = "anonymous",
            LogonName = "anonymous"
        };

        /// <summary>指定された記事に対して編集する権利を所持しているか判断します。</summary>
        /// <param name="target">記事</param>
        /// <returns></returns>
        public bool IsEntitledToEditItem(Item target)
        {
            if (target == null) throw new ArgumentNullException("target");

            if (target.Author == this) return true;

            if (target.Collaborators.Contains(this)) return true;

            if (this == Anonymous || this == Unknown) return false;

            if (target.IsFreeEditable) return true;

            return false;
        }

        /// <summary>指定された記事に対して削除する権利を所持しているか判断します。</summary>
        /// <param name="target">記事</param>
        /// <returns></returns>
        public bool IsEntitledToDeleteItem(Item target)
        {
            if (target == null) throw new ArgumentNullException("target");

            if (target.Author == this) return true;

            if (target.Collaborators.Where(x => x.Role == RoleType.Owner).Contains(this)) return true;

            return false;
        }

        /// <summary>指定された記事の共同編集者を設定・変更・削除する権利を所持しているか判断します。</summary>
        /// <param name="target">記事</param>
        /// <returns></returns>
        public bool IsEntitledToEditItemCollaborators(Item target)
        {
            if (target == null) throw new ArgumentNullException("target");

            if (target.Author == this) return true;

            if (target.Collaborators.Where(x => x.Role == RoleType.Owner).Contains(this)) return true;

            return false;
        }

        /// <summary>指定された記事に対してコメントを書き込む権利を所持しているか判断します。</summary>
        /// <param name="target">記事</param>
        /// <returns></returns>
        public bool IsEntitledToWriteComments(Item target)
        {
            if (target == null) throw new ArgumentNullException("target");

            if (this == Anonymous) return false;

            if (this == Unknown) return false;

            return true;
        }

        /// <summary>指定された下書きの公開範囲（限定共有や誰でも編集）を変更する権利を所持しているか判断します。</summary>
        /// <param name="target">記事</param>
        /// <returns></returns>
        public bool IsEntitledToChangeOpenRange(Draft target)
        {
            if (target == null) throw new ArgumentNullException("target");

            if (!target.IsContributed) return true;

            if (target.Author == this) return true;

            if (target.Collaborators.Where(x => x.Role == RoleType.Owner).Contains(this)) return true;

            return false;
        }
    }
}
