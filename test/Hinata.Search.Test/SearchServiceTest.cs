using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Hinata.Data;
using Hinata.Markdown;
using JavaScriptEngineSwitcher.V8;
using NUnit.Framework;

namespace Hinata.Search.Test
{
    [TestFixture]
    public class SearchServiceTest
    {
        private static readonly string _connectionString = @"Data Source=(localdb)\v11.0;Initial Catalog=Hinata_SeachTest;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";

        [TestFixtureSetUp]
        public void SetUp()
        {
            DropAllTable();
            Database.Initialize(_connectionString);
            DapperConfig.Initialize();

            MarkdownParser.RegisterJsEngineType<V8JsEngine>();

            var service = new SearchService(_connectionString);
            service.RecreateEsIndexAsync().Wait();
        }

        [Test]
        public async Task IndexItemAsyncTest()
        {
            var service =  new SearchService(_connectionString);

            var draft = Draft.NewDraft(new User("TEST"), ItemType.Article);
            draft.Title = "Title";
            draft.Body = "Body";
            draft.ItemTags.Add(new ItemTag("Tag1", "version"));
            draft.ItemTags.Add(new ItemTag("Tag2", null));

            var item = draft.ToItem(true);

            await service.IndexItemAsync(item);
        }

        [Test]
        public async Task BulkItemsAsyncTest()
        {
            var service = new SearchService(_connectionString);

            var draft1 = Draft.NewDraft(new User("TEST1"), ItemType.Article);
            draft1.Title = "雪村あおい（ゆきむら あおい）";
            draft1.Body = @"本作の主人公。2月19日生まれ。高校1年生。15歳。血液型はB型。身長152cm、体重43kgでカップサイズはAカップ。
亜麻色の、襟足ほどに切りそろえた髪型で、普段は左こめかみ付近に黒い髪飾りを着けている。
小学生の頃にジャングルジムから落ちて以来の高所恐怖症で、ロープウェイなどに乗れなかったりする。また、その頃から料理・裁縫・読書といった屋内での一人遊びが趣味になった[注 2]。
人見知りが激しく、クラスメイトからの誘いも強引な理由をつくって断ってしまう。その一方で、負けん気が強く、ひなたに挑発されて彼女の思惑に乗せられたりもする。時々、「女子力」を気にする。登山については全くの素人だが、ひなたに付き合ううち、自らもインターネットを活用して知識を得るなど、興味を抱きつつある。
ひなたに対しては、再会当初はあまり良い印象を持たず、表に出さないまでも抗議したり悪態をついたりしたが、共に過ごすうちに彼女の気配りや社交性を見直している。";
            draft1.ItemTags.Add(new ItemTag("Tag1", "version"));
            draft1.ItemTags.Add(new ItemTag("Tag2", null));

            var item1 = draft1.ToItem(true);

            var draft2 = Draft.NewDraft(new User("TEST2"), ItemType.Article);
            draft2.Title = "倉上ひなた（くらうえ ひなた）";
            draft2.Body = @"あおいと同学年の幼馴染。11月11日生まれ。15歳。血液型はA型。身長155cm、体重47kgでカップサイズはBカップ（4巻時点）→Cカップ（6巻）。
短めの黒髪を両側頭部でまとめた、いわゆるツインテールの髪型が特徴。
あおいとは中学時代は疎遠だったが、高校で偶然再会した。あおいにとってはかつてあまり愉快でなかった友達で、互いに成長して再会した現在も強引にあおいを引っ張りまわしたり、彼女を巧みに挑発して自分の計画に乗せたりするが、一方であおいのことは友達としてとても大切に思っており、時たまケンカもするが思いやりを持って接している。明るく、人に対して物怖じしない性格。大雑把に見えるが周りのことを良く見ており、繊細な一面も併せ持つ。また頭が良く、成績は学年でも上位に位置する。
父の影響で登山が大好きだが、経験や知識はまだ初心者。";
            draft2.ItemTags.Add(new ItemTag("Tag2", "version"));
            draft2.ItemTags.Add(new ItemTag("Tag3", null));

            var item2 = draft2.ToItem(true);

            var draft3 = Draft.NewDraft(new User("TEST3"), ItemType.Article);
            draft3.Title = "斉藤楓（さいとう かえで）";
            draft3.Body = @"1月16日生まれ。16歳。血液型はAB型。あおい・ひなたと同じ学校の先輩で山友達。身長164cm、体重50kgでブラのサイズはG70だが、下着店の店員が脇の肉を寄せたところ1カップ増えた。
登山用品店でシュラフ選びで迷っていたところあおいと出会う。あおいにとってひなた以外での最初の友達で、あおいの良き相談相手でもある。腰丈のロングストレートの黒髪で眼鏡をかけていて、あおいによると「背が高くスタイルも良い、かっこいい」。山登りが趣味で、一人で縦走登山[注 4]をするなどしている。ただし料理は苦手。登山費用や備品費用はアルバイト代などでやり繰りしている。
山のこと以外の日常にはあまり関心を持っていない。特にファッションには無頓着で、自室にいる時はスポーツブラにスパッツという下着姿でいる事が多く、外出する時も女の子らしくないラフないでたちが多い。あおいや友人のゆうかからは「スタイルが良いのにもったいない」と言われており、ゆうかに女の子っぽい恰好をさせられたりしている。勉強についても決して苦手ではないが切羽詰まらないと動かないタイプで、ゆうかに「本気を出してやればできるのにもったいない」と言われている。
登山に関してはもともと単独行指向だったが、あおいたちと山に登るようになってからは、あおいたちの登山の先生として自身の知識や経験を教える一方、あおいたちのことを気遣うことで自身の登山に対するスタンスも変わってきている。
小春から登山部に誘われているが、「自分のやりたい登山ではない」として断っている。";
            draft3.ItemTags.Add(new ItemTag("Tag3", "version"));
            draft3.ItemTags.Add(new ItemTag("Tag4", null));

            var item3 = draft3.ToItem(true);

            var draft4 = Draft.NewDraft(new User("TEST4"), ItemType.Article);
            draft4.Title = "青羽ここな（あおば ここな）";
            draft4.Body = @"中学2年生（13歳）の少女。8月11日生まれ。13歳。血液型はO型。身長144cm、体重38kgでカップサイズは「まだほんのり」。あおいたちの山友達。
高尾山でモモンガを探していたところ下山途中のあおい達と出会う。両親は共働きのため、家事全般が得意。ウェーブのかかった長い茶色の髪で、前髪の一部を三つ編み状に結っている。あおいによる第一印象は、森ガール。モモンガに限らずかわいいもの、特に動物全般が好き。なかでも馬には目がなく、実際の馬以外にも夢馬くん（飯能市のゆるキャラ）やぐんまちゃんといった馬をモチーフとしたゆるキャラも大好き。
華奢な体格だが体力と運動神経はあり、手先も器用。また中学生ながら頭も良く博識で、あおいたちも感心するほどの雑学の知識を披露することも。ただ、時おり妄想にふける癖がある。";
            draft4.ItemTags.Add(new ItemTag("Tag3", "version"));
            draft4.ItemTags.Add(new ItemTag("Tag4", null));

            var item4 = draft4.ToItem(true);

            await service.BulkItemsAsync(new[] {item1, item2, item3, item4});
        }

        [Test]
        public async Task SearchItemIdAsyncTest()
        {
            var service = new SearchService(_connectionString);
            var draft1 = Draft.NewDraft(new User("TEST1"), ItemType.Article);
            draft1.Title = "雪村あおい（ゆきむら あおい）";
            draft1.Body = @"本作の主人公。2月19日生まれ。高校1年生。15歳。血液型はB型。身長152cm、体重43kgでカップサイズはAカップ。
亜麻色の、襟足ほどに切りそろえた髪型で、普段は左こめかみ付近に黒い髪飾りを着けている。
小学生の頃にジャングルジムから落ちて以来の高所恐怖症で、ロープウェイなどに乗れなかったりする。また、その頃から料理・裁縫・読書といった屋内での一人遊びが趣味になった[注 2]。
人見知りが激しく、クラスメイトからの誘いも強引な理由をつくって断ってしまう。その一方で、負けん気が強く、ひなたに挑発されて彼女の思惑に乗せられたりもする。時々、「女子力」を気にする。登山については全くの素人だが、ひなたに付き合ううち、自らもインターネットを活用して知識を得るなど、興味を抱きつつある。
ひなたに対しては、再会当初はあまり良い印象を持たず、表に出さないまでも抗議したり悪態をついたりしたが、共に過ごすうちに彼女の気配りや社交性を見直している。";
            draft1.ItemTags.Add(new ItemTag("Tag1", "version"));
            draft1.ItemTags.Add(new ItemTag("Tag2", null));

            var item1 = draft1.ToItem(true);

            var draft2 = Draft.NewDraft(new User("TEST2"), ItemType.Article);
            draft2.Title = "倉上ひなた（くらうえ ひなた）";
            draft2.Body = @"あおいと同学年の幼馴染。11月11日生まれ。15歳。血液型はA型。身長155cm、体重47kgでカップサイズはBカップ（4巻時点）→Cカップ（6巻）。
短めの黒髪を両側頭部でまとめた、いわゆるツインテールの髪型が特徴。
あおいとは中学時代は疎遠だったが、高校で偶然再会した。あおいにとってはかつてあまり愉快でなかった友達で、互いに成長して再会した現在も強引にあおいを引っ張りまわしたり、彼女を巧みに挑発して自分の計画に乗せたりするが、一方であおいのことは友達としてとても大切に思っており、時たまケンカもするが思いやりを持って接している。明るく、人に対して物怖じしない性格。大雑把に見えるが周りのことを良く見ており、繊細な一面も併せ持つ。また頭が良く、成績は学年でも上位に位置する。
父の影響で登山が大好きだが、経験や知識はまだ初心者。";
            draft2.ItemTags.Add(new ItemTag("Tag2", "version"));
            draft2.ItemTags.Add(new ItemTag("Tag3", null));

            var item2 = draft2.ToItem(true);

            var draft3 = Draft.NewDraft(new User("TEST3"), ItemType.Article);
            draft3.Title = "斉藤楓（さいとう かえで）";
            draft3.Body = @"1月16日生まれ。16歳。血液型はAB型。あおい・ひなたと同じ学校の先輩で山友達。身長164cm、体重50kgでブラのサイズはG70だが、下着店の店員が脇の肉を寄せたところ1カップ増えた。
登山用品店でシュラフ選びで迷っていたところあおいと出会う。あおいにとってひなた以外での最初の友達で、あおいの良き相談相手でもある。腰丈のロングストレートの黒髪で眼鏡をかけていて、あおいによると「背が高くスタイルも良い、かっこいい」。山登りが趣味で、一人で縦走登山[注 4]をするなどしている。ただし料理は苦手。登山費用や備品費用はアルバイト代などでやり繰りしている。
山のこと以外の日常にはあまり関心を持っていない。特にファッションには無頓着で、自室にいる時はスポーツブラにスパッツという下着姿でいる事が多く、外出する時も女の子らしくないラフないでたちが多い。あおいや友人のゆうかからは「スタイルが良いのにもったいない」と言われており、ゆうかに女の子っぽい恰好をさせられたりしている。勉強についても決して苦手ではないが切羽詰まらないと動かないタイプで、ゆうかに「本気を出してやればできるのにもったいない」と言われている。
登山に関してはもともと単独行指向だったが、あおいたちと山に登るようになってからは、あおいたちの登山の先生として自身の知識や経験を教える一方、あおいたちのことを気遣うことで自身の登山に対するスタンスも変わってきている。
小春から登山部に誘われているが、「自分のやりたい登山ではない」として断っている。";
            draft3.ItemTags.Add(new ItemTag("Tag3", "version"));
            draft3.ItemTags.Add(new ItemTag("Tag4", null));

            var item3 = draft3.ToItem(true);

            var draft4 = Draft.NewDraft(new User("TEST4"), ItemType.Article);
            draft4.Title = "青羽ここな（あおば ここな）";
            draft4.Body = @"中学2年生（13歳）の少女。8月11日生まれ。13歳。血液型はO型。身長144cm、体重38kgでカップサイズは「まだほんのり」。あおいたちの山友達。
高尾山でモモンガを探していたところ下山途中のあおい達と出会う。両親は共働きのため、家事全般が得意。ウェーブのかかった長い茶色の髪で、前髪の一部を三つ編み状に結っている。あおいによる第一印象は、森ガール。モモンガに限らずかわいいもの、特に動物全般が好き。なかでも馬には目がなく、実際の馬以外にも夢馬くん（飯能市のゆるキャラ）やぐんまちゃんといった馬をモチーフとしたゆるキャラも大好き。
華奢な体格だが体力と運動神経はあり、手先も器用。また中学生ながら頭も良く博識で、あおいたちも感心するほどの雑学の知識を披露することも。ただ、時おり妄想にふける癖がある。";
            draft4.ItemTags.Add(new ItemTag("Tag3", "version"));
            draft4.ItemTags.Add(new ItemTag("Tag4", null));

            var item4 = draft4.ToItem(true);

            await service.BulkItemsAsync(new[] { item1, item2, item3, item4 });

            var ids = await service.SearchItemIdAsync(new SearchCondition {KeyWords = {"登山部 眼鏡"}});
        }

        private static void DropAllTable()
        {
            var sb = new SqlConnectionStringBuilder(_connectionString);
            var initialCatalog = sb.InitialCatalog;
            sb.InitialCatalog = "master";
            var cn = new SqlConnection(sb.ToString());
            try
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"select * from sys.databases where name = '{0}'", initialCatalog);
                    cmd.CommandType = CommandType.Text;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Database.MigrateDown(_connectionString);
                        }
                    }
                }
            }
            catch
            {
                // do nothing.
            }
            finally
            {
                cn.Dispose();
            }
        }
    }
}
