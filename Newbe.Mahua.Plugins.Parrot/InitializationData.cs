using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;
using System.Collections.Generic;

namespace Newbe.Mahua.Plugins.Parrot
{
    class InitializationData
    {
        GenerateTableHelper generate = null;
        Dictionary<int, string> dic = null;
        public InitializationData()
        {
            int i = 0;
            generate = new GenerateTableHelper();
            dic = new Dictionary<int, string>();
            dic.Add(i++, "筑基");
            dic.Add(i++, "开光");
            dic.Add(i++, "融合");
            dic.Add(i++, "金丹");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");
            dic.Add(i++, "筑基");

        }
        public void InitializationQQXXZM()
        {
            //generate.StructureSql<QQXXZM>().SubmitSqlServer();
            QQXXZM xxzm = new QQXXZM();
            IList<QQXXZM> list = new List<QQXXZM>();
            xxzm.QQXXZM_NAME = "点苍派";
            xxzm.QQXXZM_HH = true;
            xxzm.QQXXZM_LEVEL = "开光期";
            xxzm.QQXXZM_JS = "这是正规门派";
            list.Add(xxzm);
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
