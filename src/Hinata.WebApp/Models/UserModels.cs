using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Hinata.Web.Mvc.DataAnnotations;

namespace Hinata.Models
{
    public class UserCreateModel
    {
        [Required]
        [PlaceHolder("一度設定すると変更できません。メールアドレスのローカル部など、他の人と重複しないようなIDを設定して下さい。例）yamada_t")]
        [Remote("CheckName", "User")]
        [StringLength(20, ErrorMessage = "{0} の長さは {1} までです。")]
        [RegularExpression(@"[0-9a-zA-Z-_]+", ErrorMessage = "半角英数とハイフン、アンダーバーのみ入力できます。")]
        [DisplayName("ユーザーID")]
        public string Name { get; set; }

        [Required]
        [PlaceHolder("山田 太郎")]
        [StringLength(20, ErrorMessage = "{0} の長さは {1} までです。")]
        [DisplayName("表示名")]
        public string DisplayName { get; set; }

        [PlaceHolder("アイコン画像のあるURLを指定して下さい。例）http://xxx/icon/xxx.png")]
        [StringLength(2048, ErrorMessage = "{0} の長さは {1} までです。")]
        [DisplayName("アイコンURL")]
        public string IconUrl { get; set; }
    }

    public class UserUpdateModel
    {
        public string Id { get; set; }

        [Required]
        [PlaceHolder("山田 太郎")]
        [StringLength(20, ErrorMessage = "{0} の長さは {1} までです。")]
        [DisplayName("表示名")]
        public string DisplayName { get; set; }

        [PlaceHolder("アイコン画像のあるURLを指定して下さい。例）http://xxx/icon/xxx.png")]
        [StringLength(2048, ErrorMessage = "{0} の長さは {1} までです。")]
        [DisplayName("アイコンURL")]
        public string IconUrl { get; set; }
    }

    public class UserIndexModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<ItemIndexModel> Items { get; private set; }

        public UserIndexModel()
        {
            Items = new List<ItemIndexModel>();
        }
    }

    public class MyPageModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<ItemIndexModel> PublicItems { get; private set; }

        public List<ItemIndexModel> PrivateItems { get; private set; }

        public MyPageModel()
        {
            PublicItems = new List<ItemIndexModel>();
            PrivateItems = new List<ItemIndexModel>();
        }
    }
}