using Less.Html;
using Nancy;
using Nancy.ModelBinding;
using Newbe.Mahua.HttpApiClient.Api;
using Newbe.Mahua.HttpApiClient.Model;
using Newbe.Mahua.Plugins.Parrot.Entity;
using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Newbe.Mahua.Plugins.Parrot
{
    public class MahuaHttpModule : NancyModule
    {
        static DateTime NowTime = DateTime.MinValue;
        CqpApi cqpApi = new CqpApi("http://127.0.0.1:36524");
        public MahuaHttpModule() : base("/api")
        {
            //MRJB();
            //cd 三秒
            if ((DateTime.Now - NowTime).TotalSeconds <= 3)
                return;
            else
                NowTime = DateTime.Now;
            Post["/ReceiveMahuaOutput"] = parameters =>
            {
                string message = string.Empty;
                BodyModel body = this.Bind<BodyModel>();
                if (body.FromQQ != 1401210070)
                    return message;
                //个人消息
                if (body != null && body.TypeCode == "ProcessPrivateMessage" && body.Platform == 0)
                {
                    switch (body.Msg)
                    {
                        case "你好":
                            body.Msg = "你也好呀";
                            break;
                        default:
                            body.Msg = body.Msg;
                            break;
                    }
                    if (!string.IsNullOrWhiteSpace(body.Msg))
                    {
                        cqpApi.Apiv1CqpCQSendPrivateMsg(new CqpCQSendPrivateMsgHttpInput
                        {
                            Msg = body.Msg,
                            Qqid = body.FromQQ
                        });
                        message = "ok";
                    }
                }
                //群消息
                else if (body != null && body.TypeCode == "ProcessGroupMessage" && body.Platform == 0)
                {
                    body.Message = StartProcessing(body.FromQQ, body.FromGroup, body.Message.Trim());
                    if (!string.IsNullOrWhiteSpace(body.Message))
                    {
                        cqpApi.Apiv1CqpCQSendGroupMsg(new CqpCQSendGroupMsgHttpInput
                        {
                            Msg = string.Format("[CQ:at,qq={0}]{1}", body.FromQQ, body.Message),
                            群号 = body.FromGroup
                        });
                        message = "ok";
                    }
                }
                return message == string.Empty ? "nothing" : message;
            };
        }

        /// <summary>
        /// 开始加工代码
        /// </summary>
        /// <param name="qqid">qq号</param>
        /// <param name="qqqid">qq群号</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public string StartProcessing(long qqid, long qqqid, string message)
        {
            if (message == "日常")
            {
                message = JX3RCSearch();
            }
            if (message.Contains("开服"))
            {
                message = message.Split(new char[2] { '开', '服' })[message.Split(new char[2] { '开', '服' }).Length - 1];
                message = JX3KFSearch(message.Trim());
            }
            else if (message == "签到")
            {
                message = QQQQD(qqid, qqqid);
            }
            else if (message == "排名")
            {
                message = QQPM(qqid, qqqid);
            }
            else if (message == "修仙")
            {
                message = QQXX(qqid, qqqid, message);
            }
            else
            {
                message = string.Empty;
            }
            return message;
        }

        /// <summary>
        /// 每日简报
        /// </summary>
        /// <returns></returns>
        public string MRJB()
        {
            string baseUrl = "https://weixin.sogou.com";
            string searchUrl = string.Format("https://weixin.sogou.com/weixin?type=2&s_from=input&query={0}+%E6%AF%8F%E6%97%A5%E7%AE%80%E6%8A%A5+%E5%BC%82%E6%AC%A1%E5%85%83%E5%B0%8F%E8%B5%84%E8%AE%AF&ie=utf8&_sug_=n&_sug_type_=&w=01019900&sut=2706&sst0=1579510352592&lkt=1%2C1579510352490%2C1579510352490", DateTime.Now.Date);
            var pageHtml = HttpGet(searchUrl, string.Empty);
            searchUrl = "https://mp.weixin.qq.com/s?src=11&timestamp=1579510352&ver=2107&signature=xHSaL3RvFxqnWtPfbSV1sbcVB09nB*SoIFaPPzpB2VfevrHHO8OSJG4FIWTb4Xc17gq8mBhA7m5OQS48Ic0ayGtiOMvU3mV9Qn-TZC-ZZG3sQZNONSFTk7ZPewPTU7-m&new=1";

            var document = HtmlParser.Parse(pageHtml);
            var bodyUrl = baseUrl + document.getElementById("sogou_vr_11002601_title_0").getAttribute("href");
            pageHtml = HttpGet(bodyUrl, string.Empty);
            var bodyHtml = document = HtmlParser.Parse(pageHtml);

            return string.Empty;
        }

        public string QQXX(long qqid, long qqqid, string msg)
        {
            string message = string.Empty;
            var qqxxhelper = new QQXXProgram(qqid, qqqid, msg);
            if (qqxxhelper.GetQQXXUserState() == QQUSER.State.NOSTATE)
            {
                var res = qqxxhelper.QQXXLogin();
                if (res == null)
                    return message;
                //如果登录成功 则添加到十分钟就退出的集合里
                QQXXProgram.logoutList.Add(new Logout
                {
                    LoginTime = DateTime.Now,
                    QQQID = qqqid,
                    QQID = qqid
                });

                message = qqxxhelper.GetUserCurrent();
            }
            else
            {
                switch (msg)
                {
                    default:
                        message = msg;
                        break;
                }
            }

            return message;
        }

        /// <summary>
        /// 剑网三日常查询
        /// </summary>
        /// <returns></returns>
        public string JX3RCSearch()
        {
            return string.Empty;
        }

        /// <summary>
        /// 剑网三开服查询
        /// </summary>
        /// <returns>返回查询结果</returns>
        public string JX3KFSearch(string serverName)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            {
                return "请输入服务器名称，查询方式是：\"开服梦江南\"";
            }
            var res = HttpGet(@"http://jx3hdws.autoupdate.kingsoft.com/jx3hd/zhcn_hd/serverlist/serverlist.ini", string.Empty);
            var resSplit = res.Split('\n');
            Dictionary<string, int> dic = new Dictionary<string, int>();
            for (int i = 0; i < resSplit.Length; i++)
            {
                var item = resSplit[i].Split('\t');
                if (!dic.ContainsKey(item[1]))
                    dic.Add(item[1], int.Parse(item[2]));
            }
            if (!dic.TryGetValue(serverName, out int resCode))
            {
                return "没有这个服务器，查询方式是：\"开服梦江南\"";
            }
            switch (resCode)
            {
                case 8:
                    serverName += "已开服！当前服务器状态：火爆";
                    break;
                case 7:
                    serverName += "已开服！当前服务器状态：拥挤";
                    break;
                case 6:
                    serverName += "已开服！当前服务器状态：正常";
                    break;
                default:
                    serverName += "未开服！";
                    break;
            }
            return serverName;
        }

        /// <summary>
        /// 字符串编码转换
        /// </summary>
        /// <param name="srcEncoding">原编码</param>
        /// <param name="dstEncoding">目标编码</param>
        /// <param name="srcBytes">原字符串</param>
        /// <returns>字符串</returns>
        public string TransferEncoding(Encoding srcEncoding, Encoding dstEncoding, string srcStr)
        {
            byte[] srcBytes = srcEncoding.GetBytes(srcStr);
            byte[] bytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(bytes);
        }
        /// <summary>
        /// QQ群排名查询
        /// </summary>
        /// <returns></returns>
        public string QQPM(long qqid, long qqqid)
        {
            string message = string.Empty;
            var res = EntityHelper.SearchSQL<QQUSER>(string.Format("SELECT TOP 10 FROM {0} ORDER BY {1} DESC; ", nameof(QQUSER), nameof(QQUSER.QQUSER_EXPERIENCE)));
            foreach (var item in res)
            {
                //item.QQUSER_QQNAME
            }
            return message;
        }
        /// <summary>
        /// QQ群签到
        /// </summary>
        /// <returns></returns>
        public string QQQQD(long qqid, long qqqid)
        {
            string message = string.Empty;

            var user = EntityHelper.Get<QQUSER>(p => p.QQUSER_QQID == qqid && p.QQUSER_QQQID == qqqid);
            Random random = new Random();
            var num = random.Next(1, 100);
            if (user == null || user.ID == Guid.Empty)
            {
                user = new QQUSER();
                //如果不存在
                user.QQUSER_QQID = qqid;
                user.QQUSER_QQQID = qqqid;
                user.QQUSER_EXPERIENCE = num;
                user.Insert();
            }
            else
            {
                if (DateTime.Now.Day == user.UPDATETIME.Day)
                {
                    return message = string.Format(" 你今天已经签到过了，请不要重复签到！当前经验值为：{0}。", user.QQUSER_EXPERIENCE);
                }
                else
                {
                    user.QQUSER_EXPERIENCE += num;
                    user.Update();
                }
            }

            return message = string.Format(" 签到成功，获得经验值：{0}，当前经验值为：{1}", num, user.QQUSER_EXPERIENCE);
        }
        public string HttpGet(string Url, string postDataStr)
        {
            try
            {
                string html;
                HttpWebRequest Web_Request = (HttpWebRequest)WebRequest.Create(Url);
                Web_Request.Timeout = 30000;
                Web_Request.Accept = "*/*";
                Web_Request.Method = "GET";
                Web_Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:72.0) Gecko/20100101 Firefox/72.0";
                Web_Request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                Web_Request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2");
                //Web_Request.Credentials = CredentialCache.DefaultCredentials;

                //设置代理属性WebProxy-------------------------------------------------
                //WebProxy proxy = new WebProxy("111.13.7.120", 80);
                ////在发起HTTP请求前将proxy赋值给HttpWebRequest的Proxy属性
                //Web_Request.Proxy = proxy;

                HttpWebResponse Web_Response = (HttpWebResponse)Web_Request.GetResponse();

                if (Web_Response.ContentEncoding.ToLower() == "gzip")       // 如果使用了GZip则先解压
                {
                    using (Stream Stream_Receive = Web_Response.GetResponseStream())
                    {
                        using (var Zip_Stream = new GZipStream(Stream_Receive, CompressionMode.Decompress))
                        {
                            using (StreamReader Stream_Reader = new StreamReader(Zip_Stream, Encoding.UTF8))
                            {
                                html = Stream_Reader.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    using (Stream Stream_Receive = Web_Response.GetResponseStream())
                    {
                        using (StreamReader Stream_Reader = new StreamReader(Stream_Receive, Encoding.Default))
                        {
                            html = Stream_Reader.ReadToEnd();
                        }
                    }
                }
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine("网站出错，{0}", ex.Message);
            }

            return "网站出错";
        }

    }

    public class BodyModel
    {
        /// <summary>
        /// QQ号
        /// </summary>
        public long FromQQ { get; set; }
        /// <summary>
        /// 私聊消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// QQ群号
        /// </summary>
        public long FromGroup { get; set; }
        /// <summary>
        /// 群聊消息
        /// </summary>
        public string Message { get; set; }
        public string TypeCode { get; set; }
        public int Platform { get; set; }
        public int SubType { get; set; }
        public int MsgId { get; set; }
        public int Font { get; set; }
        public DateTime CreateTime { get; set; }
        public string FromAnonymous { get; set; }
    }
}