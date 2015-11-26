using System;
using System.Linq;
using System.Threading.Tasks;
using Hinata.Exceptions;
using NUnit.Framework;

namespace Hinata.Data.Commands
{
    [TestFixture]
    public class ItemDbCommandTest : DbCommnandTestBase
    {
        private readonly User _author = new User("Collabo\\Author") { Name = "author", DisplayName = "作成者" };
        private readonly User _collaborator1 = new User("Collabo\\User1") { Name = "collaborator1", DisplayName = "共同編集者1" };
        private readonly User _collaborator2 = new User("Collabo\\User2") { Name = "collaborator2", DisplayName = "共同編集者2" };
        private readonly User _collaborator3 = new User("Collabo\\User3") { Name = "collaborator3", DisplayName = "共同編集者3" };

        public override void SetUp()
        {
            base.SetUp();

            UserDbCommand.SaveAsync(_author).Wait();
            UserDbCommand.SaveAsync(_collaborator1).Wait();
            UserDbCommand.SaveAsync(_collaborator2).Wait();
            UserDbCommand.SaveAsync(_collaborator3).Wait();
        }

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

            var savedDraft = await DraftDbCommand.FindAsync(draft.Id, LogonUser);

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

            var updateDraft = item.ToDraft(LogonUser);
            updateDraft.CurrentRevisionNo.Is(0);

            updateDraft.Title = "title 正常系_作成と修正 公開 修正";
            updateDraft.Body = "body 正常系_作成と修正 公開 修正";
            updateDraft.Comment = "変更";

            await DraftDbCommand.SaveAsync(updateDraft);
            var updatedDraft = await DraftDbCommand.FindAsync(updateDraft.Id, LogonUser);
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

        [Test]
        public async Task 正常系_共同編集者の追加()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "共同編集テスト";
            draft.Body = "共同編集テスト";

            var item = draft.ToItem(true);

            await ItemDbCommand.SaveAsync(item);

            // testing add collaborators
            var collaborator1 = new Collaborator(_collaborator1) {Role = RoleType.Owner};
            var collaborator2 = new Collaborator(_collaborator2) { Role = RoleType.Member };
            var collaborators = new[] {collaborator1, collaborator2};

            await ItemDbCommand.SaveCollaboratorsAsync(item, collaborators);

            var savedItem = await ItemDbCommand.FindAsync(item.Id);

            item.IsStructuralEqual(savedItem);
            savedItem.Collaborators.ToArray().IsStructuralEqual(collaborators);
        }

        [Test]
        public async Task 正常系_共同編集者の変更()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "共同編集テスト_共同編集者の変更";
            draft.Body = "共同編集テスト_共同編集者の変更";

            var item = draft.ToItem(true);

            await ItemDbCommand.SaveAsync(item);

            var collaborator1 = new Collaborator(_collaborator1) { Role = RoleType.Owner };
            var collaborator2 = new Collaborator(_collaborator2) { Role = RoleType.Member };
            var collaborators = new[] { collaborator1, collaborator2 };

            await ItemDbCommand.SaveCollaboratorsAsync(item, collaborators);

            var collaborator3 = new Collaborator(_collaborator3) { Role = RoleType.Member };
            var newCollaborators = new[] { collaborator2, collaborator3 };

            await ItemDbCommand.SaveCollaboratorsAsync(item, newCollaborators);

            var savedItem = await ItemDbCommand.FindAsync(item.Id);

            item.IsStructuralEqual(savedItem);
            savedItem.Collaborators.ToArray().IsStructuralEqual(newCollaborators);
        }

        [Test]
        public async Task 正常系_共同編集者を一人だけ追加()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "共同編集テスト_共同編集者を一人だけ追加";
            draft.Body = "共同編集テスト_共同編集者を一人だけ追加";

            var item = draft.ToItem(true);
            await ItemDbCommand.SaveAsync(item);

            var collaborator1 = new Collaborator(_collaborator1) { Role = RoleType.Owner };
            var collaborator2 = new Collaborator(_collaborator2) { Role = RoleType.Member };
            var collaborators = new[] { collaborator1, collaborator2 };
            
            await ItemDbCommand.SaveCollaboratorsAsync(item, collaborators);

            var collaborator3 = new Collaborator(_collaborator3) { Role = RoleType.Member };
            await ItemDbCommand.AddCollaboratorAsync(item, collaborator3);

            var savedItem = await ItemDbCommand.FindAsync(item.Id);

            item.IsStructuralEqual(savedItem);

            savedItem.Collaborators.ToArray().IsStructuralEqual(new[] { collaborator1, collaborator2, collaborator3 });
        }

        [Test]
        public async Task 正常系_共同編集者を一人だけ除外()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "共同編集テスト_共同編集者を一人だけ除外";
            draft.Body = "共同編集テスト_共同編集者を一人だけ除外";

            var item = draft.ToItem(true);
            await ItemDbCommand.SaveAsync(item);

            var collaborator1 = new Collaborator(_collaborator1) { Role = RoleType.Owner };
            var collaborator2 = new Collaborator(_collaborator2) { Role = RoleType.Member };
            var collaborator3 = new Collaborator(_collaborator3) { Role = RoleType.Member };
            var collaborators = new[] { collaborator1, collaborator2, collaborator3 };
            await ItemDbCommand.SaveCollaboratorsAsync(item, collaborators);

            await ItemDbCommand.RemoveCollaboratorAsync(item, collaborator1);

            var savedItem = await ItemDbCommand.FindAsync(item.Id);

            item.IsStructuralEqual(savedItem);

            savedItem.Collaborators.ToArray().IsStructuralEqual(new[] { collaborator2, collaborator3 });
        }

        [Test]
        public async Task 正常系_共同編集者のロールを変更()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "共同編集テスト_共同編集者のロールを変更";
            draft.Body = "共同編集テスト_共同編集者のロールを変更";

            var item = draft.ToItem(true);
            await ItemDbCommand.SaveAsync(item);

            var collaborator1 = new Collaborator(_collaborator1) { Role = RoleType.Member };
            var collaborator2 = new Collaborator(_collaborator2) { Role = RoleType.Member };
            var collaborator3 = new Collaborator(_collaborator3) { Role = RoleType.Member };
            var collaborators = new[] { collaborator1, collaborator2, collaborator3 };
            await ItemDbCommand.SaveCollaboratorsAsync(item, collaborators);

            collaborator1.Role = RoleType.Owner;
            await ItemDbCommand.SaveCollaboratorsAsync(item);

            var savedItem = await ItemDbCommand.FindAsync(item.Id);

            item.IsStructuralEqual(savedItem);

            savedItem.Collaborators.ToArray().IsStructuralEqual(new[] { collaborator1, collaborator2, collaborator3 });
        }

        [Test]
        public async Task 正常系_共同編集者が記事を更新()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "共同編集テスト_共同編集者が記事を更新";
            draft.Body = "共同編集テスト_共同編集者が記事を更新";

            var item = draft.ToItem(true);
            await ItemDbCommand.SaveAsync(item);
            var collaborator1 = new Collaborator(_collaborator1) { Role = RoleType.Owner };
            var collaborator2 = new Collaborator(_collaborator2) { Role = RoleType.Member };
            var collaborators = new[] { collaborator1, collaborator2 };
            await ItemDbCommand.SaveCollaboratorsAsync(item, collaborators);

            var newDraft = item.ToDraft(_collaborator1);

            newDraft.Title = "共同編集テスト_共同編集者が記事を更新した";
            newDraft.Body = "共同編集テスト_共同編集者が記事を更新した";

            var newItem = newDraft.ToItem(true);

            await ItemDbCommand.SaveAsync(newItem);

            newItem.Author.Is(_author);
            newItem.Editor.Is(_collaborator1);

            var savedItem = await ItemDbCommand.FindAsync(newItem.Id);

            savedItem.IsStructuralEqual(newItem);
        }

        [Test]
        public async Task 正常系_他の人が編集中の下書きを別の人が呼び出して記事を更新()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "共同編集テスト_他の人が編集中の下書きを別の人が呼び出して記事を更新";
            draft.Body = "共同編集テスト_他の人が編集中の下書きを別の人が呼び出して記事を更新";

            var item = draft.ToItem(true);
            await ItemDbCommand.SaveAsync(item);
            var collaborator1 = new Collaborator(_collaborator1) { Role = RoleType.Owner };
            var collaborator2 = new Collaborator(_collaborator2) { Role = RoleType.Member };
            var collaborator3 = new Collaborator(_collaborator3) { Role = RoleType.Member };
            var collaborators = new[] { collaborator1, collaborator2, collaborator3 };
            await ItemDbCommand.SaveCollaboratorsAsync(item, collaborators);

            var collaborator1Draft = item.ToDraft(_collaborator1);

            collaborator1Draft.Title = "共同編集テスト_共同編集者①が記事を編集中";
            collaborator1Draft.Body = "共同編集テスト_共同編集者①が記事を編集中";

            await DraftDbCommand.SaveAsync(collaborator1Draft);

            // 共同編集者②が下書きを呼び出して下書きを更新
            var collaborator2Draft = await DraftDbCommand.FindAsync(item.Id, _collaborator2);
            collaborator2Draft.IsNull();
            collaborator2Draft = item.ToDraft(_collaborator2);
            collaborator2Draft.Title = "共同編集テスト_共同編集者②が記事を編集中";
            collaborator2Draft.Body = "共同編集テスト_共同編集者②が記事を編集中";

            await DraftDbCommand.SaveAsync(collaborator2Draft);

            var savedCollaborator2Draft = await DraftDbCommand.FindAsync(item.Id, _collaborator2);
            savedCollaborator2Draft.IsStructuralEqual(collaborator2Draft);

            // 共同編集者③が下書きを呼び出して下書きを更新
            var collaborator3Draft = await DraftDbCommand.FindAsync(item.Id, _collaborator3);
            collaborator3Draft.IsNull();

            collaborator3Draft = item.ToDraft(_collaborator3);
            collaborator3Draft.Title = "共同編集テスト_共同編集者③が記事を編集";
            collaborator3Draft.Body = "共同編集テスト_共同編集者③が記事を編集";

            var collaborator3Item = collaborator3Draft.ToItem();

            await ItemDbCommand.SaveAsync(collaborator3Item);

            var savedCollaborator3Item = await ItemDbCommand.FindAsync(item.Id);

            savedCollaborator3Item.Author.Is(_author);
            savedCollaborator3Item.Editor.Is(_collaborator3);
            savedCollaborator3Item.IsStructuralEqual(collaborator3Item);
        }

        [Test]
        public async Task 異常系_共同編集者以外が記事を更新()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "共同編集テスト_共同編集者以外が記事を更新";
            draft.Body = "共同編集テスト_共同編集者以外が記事を更新";

            var item = draft.ToItem(true);
            await ItemDbCommand.SaveAsync(item);
            var collaborator1 = new Collaborator(_collaborator1) { Role = RoleType.Owner };
            var collaborator2 = new Collaborator(_collaborator2) { Role = RoleType.Member };
            var collaborator3 = new Collaborator(_collaborator3) { Role = RoleType.Member };
            var collaborators = new[] { collaborator1, collaborator2 };
            await ItemDbCommand.SaveCollaboratorsAsync(item, collaborators);

            try
            {
                var newDraft = item.ToDraft(collaborator3);
            }
            catch (Exception exception)
            {
                Assert.AreEqual(exception.GetType(), typeof(NotEntitledToEditItemException));
            }
        }

        [Test]
        public async Task 正常系_誰でも編集可能()
        {
            var draft = Draft.NewDraft(_author, ItemType.Article);
            draft.Title = "正常系_誰でも編集可能";
            draft.Body = "正常系_誰でも編集可能";
            var item = draft.ToItem(true, true);

            await ItemDbCommand.SaveAsync(item);

            var savedItem = await ItemDbCommand.FindAsync(item.Id);

            savedItem.IsStructuralEqual(item);
            savedItem.IsFreeEditable.IsTrue();
        }
    }
}
