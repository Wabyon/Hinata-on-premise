using System.Threading.Tasks;
using NUnit.Framework;

namespace Hinata.Data.Commands
{
    [TestFixture]
    public class UserDbCommandTest : DbCommnandTestBase
    {

        [Test]
        public async Task 正常系_作成とIDでの取得()
        {
            var user = new User(@"TestDomain\TestUser001")
            {
                Name = "create_test_001",
                DisplayName = "作成テスト1"
            };

            await UserDbCommand.SaveAsync(user);

            var created = await UserDbCommand.FindAsync(user.Id);

            created.IsStructuralEqual(user);
        }

        [Test]
        public async Task 正常系_作成とログオン名での取得()
        {
            var user = new User(@"TestDomain\TestUser002")
            {
                Name = "create_test_002",
                DisplayName = "作成テスト2"
            };

            await UserDbCommand.SaveAsync(user);

            var created = await UserDbCommand.FindByLogonNameAsync(user.LogonName);

            created.IsStructuralEqual(user);
        }

        [Test]
        public async Task 正常系_作成とユーザー名での取得()
        {
            var user = new User(@"TestDomain\TestUser003")
            {
                Name = "create_test_003",
                DisplayName = "作成テスト3"
            };

            await UserDbCommand.SaveAsync(user);

            var created = await UserDbCommand.FindByNameAsync(user.Name);

            created.IsStructuralEqual(user);
        }

        [Test]
        public async Task 正常系_ユーザー情報の更新()
        {
            var create = new User(@"TestDomain\TestUser004")
            {
                Name = "create",
                DisplayName = "作成",
            };

            await UserDbCommand.SaveAsync(create);

            var created = await UserDbCommand.FindAsync(create.Id);

            created.Name = "update";
            created.DisplayName = "更新";

            await UserDbCommand.SaveAsync(created);

            var updated = await UserDbCommand.FindAsync(create.Id);


            updated.IsStructuralEqual(created);
        }
    }
}
