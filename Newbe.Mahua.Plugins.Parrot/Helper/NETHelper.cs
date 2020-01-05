using Newbe.Mahua.Plugins.Parrot.Model.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;

namespace Newbe.Mahua.Plugins.Parrot.Helper
{
    public static class NETHelper
    {
        public class ItemClass
        {
            public int ID { get; set; }
            public string ItemName { get; set; }
            public string PinYin { get; set; }
            public string ShortPinYin { get; set; }
            public decimal Money { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public string Remark { get; set; }
        }

        public class ItemHistory
        {
            public int ID { get; set; }
            public int ItemID { get; set; }
            public decimal Money { get; set; }
            public DateTime CreateTime { get; set; }
        }

        private static int[] pyValue = new int[]
{
-20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,
-20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,
-19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515,
-19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249,
-19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,
-19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,
-18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448,
-18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012,
-17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,
-17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,
-17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733,
-16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448,
-16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,
-16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,
-15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659,
-15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394,
-15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,
-15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,
-14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902,
-14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654,
-14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,
-14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,
-14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907,
-13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601,
-13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,
-13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,
-13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831,
-12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300,
-12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,
-11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,
-11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832,
-10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328,
-10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254
};

        private static string[] pyName = new string[]
{
"A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben",
"Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can",
"Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng",
"Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong",
"Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De",
"Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui",
"Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo",
"Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong",
"Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han",
"Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan",
"Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing",
"Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke",
"Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo",
"La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang",
"Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun",
"Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian",
"Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang",
"Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning",
"Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan",
"Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po",
"Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu",
"Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou",
"Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen",
"Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu",
"Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan",
"Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian",
"Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai",
"Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao",
"Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang",
"Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun",
"Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan",
"Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan",
"Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
};

        ///
        /// 把汉字转换成拼音(全拼)
        ///
        /// 汉字字符串 /// 转换后的拼音(全拼)字符串
        public static string GetPY(this string hzString)
        {
            // 匹配中文字符
            Regex regex = new Regex("^[\u4e00-\u9fa5]$");
            byte[] array = new byte[2];
            string pyString = "";
            int chrAsc = 0;
            int i1 = 0;
            int i2 = 0;
            char[] noWChar = hzString.ToCharArray();

            for (int j = 0; j < noWChar.Length; j++)
            {
                // 中文字符
                if (regex.IsMatch(noWChar[j].ToString()))
                {
                    array = System.Text.Encoding.Default.GetBytes(noWChar[j].ToString());
                    i1 = (short)(array[0]);
                    i2 = (short)(array[1]);
                    chrAsc = i1 * 256 + i2 - 65536;
                    if (chrAsc > 0 && chrAsc < 160)
                    {
                        pyString += noWChar[j];
                    }
                    else
                    {
                        // 修正部分文字
                        if (chrAsc == -9254) // 修正"圳"字
                            pyString += "Zhen";
                        else
                        {
                            for (int i = (pyValue.Length - 1); i >= 0; i--)
                            {
                                if (pyValue[i] <= chrAsc)
                                {
                                    pyString += pyName[i];
                                    break;
                                }
                            }
                        }
                    }
                }
                // 非中文字符
                else
                {
                    pyString += noWChar[j].ToString();
                }
            }
            return pyString;
        }

        public static string GetShortPY(this string hzString)
        {
            // 匹配中文字符
            Regex regex = new Regex("^[\u4e00-\u9fa5]$");
            byte[] array = new byte[2];
            string pyString = "";
            char[] noWChar = hzString.ToCharArray();

            for (int j = 0; j < noWChar.Length; j++)
            {
                // 中文字符
                if (regex.IsMatch(noWChar[j].ToString()))
                {
                    pyString += noWChar[j].ToString().GetPYChar();
                }
                // 非中文字符
                else
                {
                    pyString += noWChar[j].ToString();
                }
            }
            return pyString;
        }

        /// <summary> 
        /// 取单个字符的拼音声母 
        /// Code By MuseStudio@hotmail.com 
        /// 2004-11-30 
        /// </summary> 
        /// <param name="c">要转换的单个汉字</param> 
        /// <returns>拼音首字母</returns> 
        public static string GetPYChar(this string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));

            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "g";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";

            return "*";
        }

        /// <summary>
        /// 自动匹配同类型对象值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">需要设值的对象</param>
        /// <param name="model">取值的对象</param>
        /// <param name="modifyNull">修改Null值，默认否</param>
        public static void SetObjectValue<T>(this T entity) where T : IKey<Guid>
        {
            //PropertyInfo[] propertys = entity.GetType().GetProperties();
            //foreach (PropertyInfo property in propertys)
            //{
            //    if (property.CanWrite)
            //    {
            //        var value = property.GetValue(model);
            //        if (value == null && modifyNull == false)
            //            continue;
            //        if (value != null)
            //        {
            //            if (property.Name == "Id")
            //                continue;
            //        }
            //        property.SetValue(entity, value);
            //    }
            //}
        }

        /// <summary>
        /// 把dataTable转换为model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <param name="onlyOne">是否只循环一次,取第一条</param>
        /// <returns></returns>
        public static IList<T> ToListModel<T>(this DataTable dataTable, bool onlyOne = false) where T : new()
        {
            List<T> list = new List<T>();// 定义集合
            foreach (DataRow dr in dataTable.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
                foreach (var p in propertys)
                {
                    if (p.CanWrite == false) continue;
                    var value = dr[p.Name];
                    if (value != DBNull.Value)
                    {
                        if (p.PropertyType.FullName.Contains("System.Nullable"))
                        {
                            p.SetValue(t, Convert.ChangeType(value, (Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType)), null);
                        }
                        else
                        {
                            switch (p.PropertyType.FullName)
                            {
                                case "System.Decimal":
                                    p.SetValue(t, value.ToDecimal(), null);
                                    break;
                                case "System.String":
                                    p.SetValue(t, value.ToString(), null);
                                    break;
                                case "System.Char":
                                    p.SetValue(t, value.ToChar(), null);
                                    break;
                                case "System.Guid":
                                    p.SetValue(t, value.ToGuid(), null);
                                    break;
                                case "System.Int16":
                                    p.SetValue(t, value.ToInt16(), null);
                                    break;
                                case "System.Int32":
                                    p.SetValue(t, value.ToInt32(), null);
                                    break;
                                case "System.Int64":
                                    p.SetValue(t, value.ToInt64(), null);
                                    break;
                                case "System.Byte[]":
                                    p.SetValue(t, value.ToByte(), null);
                                    break;
                                case "System.Boolean":
                                    p.SetValue(t, value.ToBoolean(), null);
                                    break;
                                case "System.Double":
                                    p.SetValue(t, value.ToDouble(), null);
                                    break;
                                case "System.DateTime":
                                    p.SetValue(t, value ?? value.ToDateTime(), null);
                                    break;
                                default:
                                    if (p.PropertyType.BaseType.Name == "Enum")
                                    {
                                        p.SetValue(t, value ?? value, null);
                                        break;
                                    }
                                    else
                                        throw new Exception("类型不匹配:" + p.PropertyType.FullName);
                            }
                        }
                    }
                }
                list.Add(t);
                if (onlyOne)
                    break;
            }
            return list;
        }

        public static Int16 ToInt16(this Object value)
        {
            return Int16.Parse(value.ToString());
        }
        public static Int32 ToInt32(this Object value)
        {
            return Int32.Parse(value.ToString());
        }
        public static Int64 ToInt64(this Object value)
        {
            return Int64.Parse(value.ToString());
        }
        public static Decimal ToDecimal(this Object value)
        {
            return Decimal.Parse(value.ToString());
        }
        public static Char ToChar(this Object value)
        {
            return Char.Parse(value.ToString());
        }
        public static Guid ToGuid(this Object value)
        {
            return Guid.Parse(value.ToString());
        }
        public static Byte ToByte(this Object value)
        {
            return Byte.Parse(value.ToString());
        }
        public static Boolean ToBoolean(this Object value)
        {
            return Boolean.Parse(value.ToString());
        }
        public static Double ToDouble(this Object value)
        {
            return Double.Parse(value.ToString());
        }
        public static DateTime ToDateTime(this Object value)
        {
            return DateTime.Parse(value.ToString());
        }
        public static string ToEnum(this Object value, Type enumType)
        {
            return Enum.Format(enumType, value, "d");
        }

        public static StringBuilder AppendLine(this StringBuilder stringBuilder, string text, params object[] paramsObject)
        {
            return stringBuilder.AppendLine(string.Format(text, paramsObject));
        }
    }
}
