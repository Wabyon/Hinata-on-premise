using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hinata.Data.Commands
{
    [TestFixture]
    public class DraftDbCommandTest : DbCommnandTestBase
    {
        [Test]
        public async Task 正常系_記事_作成とIDでの取得_複数タグ()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Article);
            draft.Title = @"正常系_記事_作成とIDでの取得_複数タグ";
            draft.Body = "body";

            draft.ItemTags.Add(new ItemTag("tag1", "1.0.0"));
            draft.ItemTags.Add(new ItemTag("tag2", null));
            draft.ItemTags.Add(new ItemTag("tag5", null));
            draft.ItemTags.Add(new ItemTag("tag4", "2012"));

            await DraftDbCommand.SaveAsync(draft);

            var created = await DraftDbCommand.FindAsync(draft.Id);

            created.IsStructuralEqual(draft);
        }

        [Test]
        public async Task 正常系_記事_作成とIDでの取得_タグ一つ()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Article);
            draft.Title = @"正常系_記事_作成とIDでの取得_タグ一つ";
            draft.Body = "body";

            draft.ItemTags.Add(new ItemTag("tag1", "1.0.0"));

            await DraftDbCommand.SaveAsync(draft);

            var created = await DraftDbCommand.FindAsync(draft.Id);

            created.IsStructuralEqual(draft);
        }

        [Test]
        public async Task 正常系_記事_作成とIDでの取得_タグなし()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Article);
            draft.Title = @"正常系_記事_作成とIDでの取得_タグなし";
            draft.Body = "body";

            await DraftDbCommand.SaveAsync(draft);

            var created = await DraftDbCommand.FindAsync(draft.Id);

            created.IsStructuralEqual(draft);
        }

        [Test]
        public async Task 正常系_記事_作者での抽出()
        {
            var author = new User(@"TestDomain\GetByAuthorTest") { Name = "GetByAuthorTest", DisplayName = "GetByAuthorTest" };
            await UserDbCommand.SaveAsync(author);

            await DraftDbCommand.DeleteByAuthorAsync(author);

            var draft1 = Draft.NewDraft(author, ItemType.Article);
            draft1.Title = "title1";
            draft1.LastModifiedDateTime = new DateTime(2015, 1, 1);

            var draft2 = Draft.NewDraft(author, ItemType.Article);
            draft2.Title = "title2";
            draft2.LastModifiedDateTime = new DateTime(2015, 2, 1);

            var draft3 = Draft.NewDraft(author, ItemType.Article);
            draft3.Title = "title3";
            draft3.LastModifiedDateTime = new DateTime(2015, 3, 1);

            var draft4 = Draft.NewDraft(author, ItemType.Article);
            draft4.Title = "title4";
            draft4.LastModifiedDateTime = new DateTime(2015, 4, 1);

            await DraftDbCommand.SaveAsync(draft1);
            await DraftDbCommand.SaveAsync(draft2);
            await DraftDbCommand.SaveAsync(draft3);
            await DraftDbCommand.SaveAsync(draft4);

            var registered = await DraftDbCommand.GetByAuthorAsync(author);

            registered.IsStructuralEqual(new[] { draft4, draft3, draft2, draft1 });
        }

        [Test]
        public async Task 正常系_記事_作成と更新()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Article);
            draft.Title = "title 正常系_記事_作成と更新";
            draft.Body = "body 正常系_記事_作成と更新";
            draft.ItemTags.Add(new ItemTag("tag1", "1.0.0"));
            draft.ItemTags.Add(new ItemTag("tag2", "1.0.0"));

            await DraftDbCommand.SaveAsync(draft);

            var created = await DraftDbCommand.FindAsync(draft.Id);
            created.IsStructuralEqual(draft);

            created.Title = "title 正常系_記事_作成と更新 変更";
            created.Body = "body 正常系_記事_作成と更新 変更";
            created.LastModifiedDateTime = DateTime.Now;
            created.ItemTags.Clear();
            created.ItemTags.Add(new ItemTag("tag2", null));
            created.ItemTags.Add(new ItemTag("tag3", "3.0.0"));

            await DraftDbCommand.SaveAsync(created);

            var updated = await DraftDbCommand.FindAsync(created.Id);
            updated.IsStructuralEqual(created);
            updated.IsNotStructuralEqual(draft);
        }

        [Test]
        public async Task 正常系_記事_作成と削除()
        {
            var draft1 = Draft.NewDraft(LogonUser, ItemType.Article);
            draft1.Title = "title 正常系_記事_作成と削除 削除対象";
            draft1.Body = "body 正常系_記事_作成と削除 削除対象";
            draft1.ItemTags.Add(new ItemTag("tag1", "1.0.0"));
            draft1.ItemTags.Add(new ItemTag("tag2", null));

            var draft2 = Draft.NewDraft(LogonUser, ItemType.Article);
            draft2.Title = "title 正常系_記事_作成と削除 削除しない";
            draft2.Body = "title 正常系_記事_作成と削除 削除しない";
            draft2.ItemTags.Add(new ItemTag("tag3", "1.0.0"));
            draft2.ItemTags.Add(new ItemTag("tag4", null));

            await DraftDbCommand.SaveAsync(draft1);
            await DraftDbCommand.SaveAsync(draft2);

            var created1 = await DraftDbCommand.FindAsync(draft1.Id);
            created1.IsStructuralEqual(draft1);
            var created2 = await DraftDbCommand.FindAsync(draft2.Id);
            created2.IsStructuralEqual(draft2);

            await DraftDbCommand.DeleteAsync(created1.Id);

            var deleted = await DraftDbCommand.FindAsync(draft1.Id);
            deleted.IsNull();

            var undeleted = await DraftDbCommand.FindAsync(draft2.Id);
            undeleted.IsNotNull();
            undeleted.IsStructuralEqual(draft2);
        }
    }
}
