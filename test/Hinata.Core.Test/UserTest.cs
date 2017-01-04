using NUnit.Framework;

namespace Hinata
{
    [TestFixture]
    public class UserTest
    {
        private readonly User _author = new User(@"TEST\AUTHOR");
        private readonly User _ownerCollaborator = new User(@"TEST\OWNER");
        private readonly User _memberCollaborator = new User(@"TEST\MEMBER");
        private readonly User _otherUser = new User(@"TEST\OTHER");

        [Test]
        public void 通常記事の権限テスト()
        {
            var draft = Draft.NewDraft(_author);
            var item = draft.ToItem(true);

            _author.IsEntitledToEditItem(item).IsTrue("作者は編集権限がある");
            _otherUser.IsEntitledToEditItem(item).IsFalse("登録済ユーザーは編集権限がない");
            User.Anonymous.IsEntitledToEditItem(item).IsFalse("未登録ユーザーは編集権限がない");

            _author.IsEntitledToDeleteItem(item).IsTrue("作者は削除権限がある");
            _otherUser.IsEntitledToDeleteItem(item).IsFalse("登録済ユーザーは削除権限がない");
            User.Anonymous.IsEntitledToDeleteItem(item).IsFalse("未登録ユーザーは削除権限がない");

            _author.IsEntitledToWriteComments(item).IsTrue("作者はコメントを書く権限がある");
            _otherUser.IsEntitledToWriteComments(item).IsTrue("登録済ユーザーはコメントを書く権限がある");
            User.Anonymous.IsEntitledToWriteComments(item).IsFalse("未登録ユーザーはコメントを書く権限がある");
        }

        [Test]
        public void 共同編集権限テスト()
        {
            var draft = Draft.NewDraft(_author);
            var item = draft.ToItem(true);

            item.AddCollaborator(new Collaborator(_ownerCollaborator) {Role = RoleType.Owner});
            item.AddCollaborator(new Collaborator(_memberCollaborator) {Role = RoleType.Member});

            _author.IsEntitledToEditItem(item).IsTrue("作者は編集権限がある");
            _ownerCollaborator.IsEntitledToEditItem(item).IsTrue("OWNERは編集権限がある");
            _memberCollaborator.IsEntitledToEditItem(item).IsTrue("MEMBERは編集権限がある");
            _otherUser.IsEntitledToEditItem(item).IsFalse("その他のユーザーは編集権限がない");

            _author.IsEntitledToDeleteItem(item).IsTrue("作者は削除権限がある");
            _ownerCollaborator.IsEntitledToDeleteItem(item).IsTrue("OWNERは削除権限がある");
            _memberCollaborator.IsEntitledToDeleteItem(item).IsFalse("MEMBERは削除権限がない");
            _otherUser.IsEntitledToDeleteItem(item).IsFalse("その他のユーザーは削除権限がない");

            _author.IsEntitledToEditItemCollaborators(item).IsTrue("作者は共同編集者を編集する権限がある");
            _ownerCollaborator.IsEntitledToEditItemCollaborators(item).IsTrue("OWNERは共同編集者を編集する権限がある");
            _memberCollaborator.IsEntitledToEditItemCollaborators(item).IsFalse("MEMBERは共同編集者を編集する権限がない");
            _otherUser.IsEntitledToEditItemCollaborators(item).IsFalse("その他のユーザーは共同編集者を編集する権限がない");
        }
    }
}
