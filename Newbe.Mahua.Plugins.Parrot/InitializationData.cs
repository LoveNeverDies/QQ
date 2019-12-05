using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;
using System.Collections.Generic;

namespace Newbe.Mahua.Plugins.Parrot
{
    class InitializationData
    {
        GenerateTableHelper generate = null;
        public InitializationData()
        {
            int i = 0;
            generate = new GenerateTableHelper();
        }
        public void InitializationQQXXLEVEL()
        {
            //generate.StructureSql<QQXXLEVEL>().SubmitSqlServer();
            #region 添加境界
            int i = 0;
            QQXXLEVEL qq = new QQXXLEVEL();
            IList<QQXXLEVEL> list = new List<QQXXLEVEL>();
            qq.QQXXLEVEL_NAMEJD = "筑基";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "开光";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "融合";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "心动";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "金丹";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "元婴";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "出窍";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "分神";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "合体";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "洞虚";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "大乘";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "渡劫飞升";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);



            qq.QQXXLEVEL_NAMEJD = "游仙";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "真仙";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "金仙";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "玄仙";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "太乙玉仙";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "大罗金仙";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "仙尊";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "仙帝";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);



            qq.QQXXLEVEL_NAMEJD = "破碎虚空";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "空玄";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "万劫";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "混元";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "圣人";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "不朽";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "灵尊";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "天道";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            qq.QQXXLEVEL_NAMEJD = "永恒";
            qq.QQXXLEVEL_NUMBERJD = ++i;
            list.Add(qq);
            foreach (var item in list)
            {
                item.Insert();
            }
            #endregion

            /*筑基是将人身体机能改造，是身体适合修真，可以储存真元！达到这个效果即为筑基！
　　开光，灵识出现。有点像开窍的意思！其实筑基和开光的前后排名是有争论的！
　　开光之后若是将真元操作能力提高后达到收发自如的境界就是融合了！融合之后用全身真元集中到丹田形成金丹，就是小说里常说的金丹大道！
　　其实前五个的说法有很多，如果有兴趣你玩下完美世界就知道那个境界有什么用了！
　　分神，即是将元神分离，有两种
　　说法一中是分神和出窍是一个境界，一种说法是先有出窍后又分神!出窍是元婴没有意识的分离，分神是将自身的一点意识寄托在元婴之上！
　　合体，即将初步有意识的元神和自身肉体完整结合，类似于返璞归真！
　　合体之后是渡劫，
　　渡劫，本身功力达到一定境界之后引来天劫，练其身体！经受住天劫考验后一身功力圆满身体得天劫改造已具仙人躯体，随时可以飞升天劫！*/
            /*地球境界：明劲、暗劲、化劲、气罡、丹境、地元境、天元境、人王境。
天界境界：聚灵境、阴阳境、天人境、归一境、天地法相、真祖、通天、破碎虚空、证道合道、永恒、出神、入化、开天辟地。
六道：天人道、人道、畜S道、阿修罗道（魔）、饿鬼道、地狱道
龙的修炼期：胚胎期(撒下龙种)、成形期(龙种成形)、潜伏期(潜龙勿用)、静养期(龙眠水底)、蜕变期(龙鳞将蜕)、蠕动期(龙形再变)、爬行期(龙作蛇行)、游水期(龙游于海)、飞腾期(飞龙在天/神道大典)。*/
        }
        public void InitializationQQXXZM()
        {
            //generate.StructureSql<QQXXZM>().SubmitSqlServer();
            string level = "筑基";
            bool hh = true;
            QQXXZM xxzm = new QQXXZM();
            IList<QQXXZM> list = new List<QQXXZM>();
            #region 筑基期 好宗门
            xxzm.QQXXZM_NAME = "点苍派";
            xxzm.QQXXZM_HH = true;
            xxzm.QQXXZM_LEVEL = level;
            xxzm.QQXXZM_JS = "这是正规门派";
            list.Add(xxzm);
            #endregion 
            list.Add(xxzm);
            list.Add(xxzm);
            list.Add(xxzm);
            list.Add(xxzm);
            list.Add(xxzm);
            foreach (var item in list)
            {
                item.Insert();
            }
        }
    }
}
