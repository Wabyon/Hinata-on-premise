using System.Threading.Tasks;
using NUnit.Framework;

namespace Hinata.Data.Commands
{
    [TestFixture]
    public class ItemDbCommandTest : DbCommnandTestBase
    {
        [Test]
        public async Task 正常系_作成とIDでの抽出_公開記事()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Article);
            draft.Title = "title 正常系_作成とIDでの抽出_公開記事";
            draft.Body = "body 正常系_作成とIDでの抽出_公開記事";
            draft.Tags.Add(new Tag("tag1", "1.0.0"));
            draft.Tags.Add(new Tag("tag2", null));

            var item = draft.ToItem(true);

            await ItemDbCommand.SaveAsync(item);

            var created = await ItemDbCommand.FindAsync(item.Id);

            created.IsStructuralEqual(item);
            Assert.AreEqual(draft.Id, created.Id);
            Assert.AreEqual(draft.Title, created.Title);
            Assert.AreEqual(draft.Body, created.Body);
            created.Author.IsStructuralEqual(draft.Author);
        }

        [Test]
        public async Task 正常系_作成とIDでの抽出_非公開記事()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Article);
            draft.Title = "title 正常系_作成とIDでの抽出_非公開記事";
            draft.Body = "body 正常系_作成とIDでの抽出_非公開記事";
            draft.Tags.Add(new Tag("tag1", "1.0.0"));
            draft.Tags.Add(new Tag("tag2", null));

            var item = draft.ToItem(false);

            await ItemDbCommand.SaveAsync(item);

            var created = await ItemDbCommand.FindAsync(item.Id);

            created.IsStructuralEqual(item);

            created.IsStructuralEqual(item);
            Assert.AreEqual(draft.Id, created.Id);
            Assert.AreEqual(draft.Title, created.Title);
            Assert.AreEqual(draft.Body, created.Body);
            created.Author.IsStructuralEqual(draft.Author);
        }

        [Test]
        public async Task 正常系_作成とIDでの抽出_公開質問()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Ask);
            draft.Title = "title 正常系_作成とIDでの抽出_公開質問";
            draft.Body = "body 正常系_作成とIDでの抽出_公開質問";
            draft.Tags.Add(new Tag("tag1", "1.0.0"));
            draft.Tags.Add(new Tag("tag2", null));

            var item = draft.ToItem(true);

            await ItemDbCommand.SaveAsync(item);

            var created = await ItemDbCommand.FindAsync(item.Id);

            created.IsStructuralEqual(item);

            created.IsStructuralEqual(item);
            Assert.AreEqual(draft.Id, created.Id);
            Assert.AreEqual(draft.Title, created.Title);
            Assert.AreEqual(draft.Body, created.Body);
            created.Author.IsStructuralEqual(draft.Author);
        }

        [Test]
        public async Task 正常系_作成とIDでの抽出_非公開質問()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Ask);
            draft.Title = "title 正常系_作成とIDでの抽出_非公開質問";
            draft.Body = "body 正常系_作成とIDでの抽出_非公開質問";
            draft.Tags.Add(new Tag("tag1", "1.0.0"));
            draft.Tags.Add(new Tag("tag2", null));

            var item = draft.ToItem(false);

            await ItemDbCommand.SaveAsync(item);

            var created = await ItemDbCommand.FindAsync(item.Id);

            created.IsStructuralEqual(item);

            created.IsStructuralEqual(item);
            Assert.AreEqual(draft.Id, created.Id);
            Assert.AreEqual(draft.Title, created.Title);
            Assert.AreEqual(draft.Body, created.Body);
            created.Author.IsStructuralEqual(draft.Author);
        }

        [Test]
        public async Task 正常系_公開記事_作成と修正()
        {
            var draft = Draft.NewDraft(LogonUser, ItemType.Article);
            draft.Title = "title 正常系_作成と修正";
            draft.Body = "body 正常系_作成と修正";
            draft.Tags.Add(new Tag("tag1","1.0.0"));
            draft.Tags.Add(new Tag("tag2", null));

            await DraftDbCommand.SaveAsync(draft);

            var savedDraft = await DraftDbCommand.FindAsync(draft.Id);
            savedDraft.Title = "title 正常系_作成と修正 公開";
            savedDraft.Body = "body 正常系_作成と修正 公開";

            var item = savedDraft.ToItem(true);

            await ItemDbCommand.SaveAsync(item);

            var savedItem = await ItemDbCommand.FindAsync(item.Id);
            savedItem.IsStructuralEqual(item);
            Assert.AreEqual(savedDraft.Id, savedItem.Id);
            Assert.AreEqual(savedDraft.Title, savedItem.Title);
            Assert.AreEqual(savedDraft.Body, savedItem.Body);
            savedItem.Author.IsStructuralEqual(savedDraft.Author);

            var updateDraft = item.ToDraft();
            updateDraft.Title = "title 正常系_作成と修正 公開 修正";
            updateDraft.Body = "body 正常系_作成と修正 公開 修正";

            await DraftDbCommand.SaveAsync(updateDraft);
            var updatedDraft = await DraftDbCommand.FindAsync(updateDraft.Id);
            updatedDraft.IsStructuralEqual(updateDraft);

            var updateItem = updatedDraft.ToItem();

            await ItemDbCommand.SaveAsync(updateItem);

            var updatedItem = await ItemDbCommand.FindAsync(updateItem.Id);
            updatedItem.IsStructuralEqual(updateItem);
            Assert.AreEqual(updatedDraft.Id, updatedItem.Id);
            Assert.AreEqual(updatedDraft.Title, updatedItem.Title);
            Assert.AreEqual(updatedDraft.Body, updatedItem.Body);
            updatedItem.Author.IsStructuralEqual(updatedDraft.Author);
        }
    }
}
