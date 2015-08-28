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
            draft.ItemTags.Add(new ItemTag("tag1", "1.0.0"));
            draft.ItemTags.Add(new ItemTag("tag2", null));

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
            draft.ItemTags.Add(new ItemTag("tag1", "1.0.0"));
            draft.ItemTags.Add(new ItemTag("tag2", null));

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
            draft.ItemTags.Add(new ItemTag("tag1", "1.0.0"));
            draft.ItemTags.Add(new ItemTag("tag2", null));

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
            draft.ItemTags.Add(new ItemTag("tag1", "1.0.0"));
            draft.ItemTags.Add(new ItemTag("tag2", null));

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
            draft.ItemTags.Add(new ItemTag("tag1","1.0.0"));
            draft.ItemTags.Add(new ItemTag("tag2", null));

            await DraftDbCommand.SaveAsync(draft);

            var savedDraft = await DraftDbCommand.FindAsync(draft.Id);

            savedDraft.CurrentRevisionNo.Is(-1);

            savedDraft.Title = "title 正常系_作成と修正 公開";
            savedDraft.Body = "body 正常系_作成と修正 公開";

            var item = savedDraft.ToItem(true);
            item.RevisionNo.Is(0);

            await ItemDbCommand.SaveAsync(item);

            var savedItem = await ItemDbCommand.FindAsync(item.Id);
            savedItem.IsStructuralEqual(item);
            Assert.AreEqual(savedDraft.Id, savedItem.Id);
            Assert.AreEqual(savedDraft.Title, savedItem.Title);
            Assert.AreEqual(savedDraft.Body, savedItem.Body);
            savedItem.Author.IsStructuralEqual(savedDraft.Author);
            savedItem.RevisionNo.Is(0);
            savedItem.RevisionCount.Is(1);

            var updateDraft = item.ToDraft();
            updateDraft.CurrentRevisionNo.Is(0);

            updateDraft.Title = "title 正常系_作成と修正 公開 修正";
            updateDraft.Body = "body 正常系_作成と修正 公開 修正";
            updateDraft.Comment = "変更";

            await DraftDbCommand.SaveAsync(updateDraft);
            var updatedDraft = await DraftDbCommand.FindAsync(updateDraft.Id);
            updatedDraft.IsStructuralEqual(updateDraft);
            updatedDraft.CurrentRevisionNo.Is(0);
            updatedDraft.Title.Is(updateDraft.Title);
            updatedDraft.ItemTags.IsStructuralEqual(updateDraft.ItemTags);
            updatedDraft.Body.Is(updateDraft.Body);
            updatedDraft.Comment.Is(updateDraft.Comment);
            updatedDraft.Author.IsStructuralEqual(updateDraft.Author);

            var updateItem = updatedDraft.ToItem();
            updateItem.RevisionNo.Is(1);

            await ItemDbCommand.SaveAsync(updateItem);

            var updatedItem = await ItemDbCommand.FindAsync(updateItem.Id);
            updatedItem.IsStructuralEqual(updateItem);
            Assert.AreEqual(updatedDraft.Id, updatedItem.Id);
            Assert.AreEqual(updatedDraft.Title, updatedItem.Title);
            Assert.AreEqual(updatedDraft.Body, updatedItem.Body);
            updatedItem.Author.IsStructuralEqual(updatedDraft.Author);
            updatedItem.RevisionNo.Is(1);
            updatedItem.RevisionCount.Is(2);
        }

    }
}
