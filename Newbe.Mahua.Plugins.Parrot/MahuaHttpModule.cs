using Nancy;
using Nancy.ModelBinding;
using Newbe.Mahua.HttpApiClient.Api;
using Newbe.Mahua.HttpApiClient.Model;
using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
            //cd 三秒
            if ((DateTime.Now - NowTime).TotalSeconds <= 3)
                return;
            Post["/ReceiveMahuaOutput"] = parameters =>
            {
                BodyModel body = this.Bind<BodyModel>();
                string message = string.Empty;
                Func<BodyModel, string> func = StartMahuaHttpModule;
                func.BeginInvoke(body, new AsyncCallback(p =>
                {
                    AsyncResult res = (AsyncResult)p;
                    var resFunc = (Func<BodyModel, string>)res.AsyncDelegate;
                    message = resFunc.EndInvoke(p);
                }), null);
                return message;
            };
        }

        public string StartMahuaHttpModule(BodyModel body)
        {
            string message = string.Empty;
            //个人消息
            if (body != null && body.TypeCode == "ProcessPrivateMessage" && body.Platform == 0)
            {
                switch (body.Msg)
                {
                    case "你好":
                        body.Msg = "你也好呀";
                        break;
                    default:
                        body.Msg = string.Empty;
                        break;
                }
                if (!string.IsNullOrWhiteSpace(body.Msg))
                    cqpApi.Apiv1CqpCQSendPrivateMsg(new CqpCQSendPrivateMsgHttpInput
                    {
                        Msg = body.Msg,
                        Qqid = body.FromQQ
                    }
                    );
                message = "ok";
            }
            //群消息
            else if (body != null && body.TypeCode == "ProcessGroupMessage" && body.Platform == 0)
            {
                body.Message = StartProcessing(body.FromQQ, body.FromGroup, body.Message.Trim());
                if (!string.IsNullOrWhiteSpace(body.Message))
                    cqpApi.Apiv1CqpCQSendGroupMsg(new CqpCQSendGroupMsgHttpInput
                    {
                        Msg = body.Message,
                        群号 = body.FromGroup
                    });
                message = "ok";
            }
            return message == string.Empty ? "nothing" : message;
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
            else if (message == "修仙")
            {
                message = QQXX(qqid, qqqid);
            }
            else
            {
                message = string.Empty;
            }
            return message;
        }

        public string QQXX(long qqid, long qqqid)
        {
            new QQXXProgram().QQXXLogin();
            return string.Empty;
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
        /// QQ群签到
        /// </summary>
        /// <returns></returns>
        public string QQQQD(long qqid, long qqqid)
        {
            string message = string.Empty;

            var user = EntityHelper.Get<QQUSER>(p => p.QQUSER_QQID == qqid && p.QQUSER_QQQID == qqqid);
            Random random = new Random();
            var num = random.Next(1, 100);
            if (user.ID == Guid.Empty)
            {
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
                    message = string.Format("@{0} 你今天已经签到过了，请不要重复签到！", qqid);
                }
                else
                {
                    user.QQUSER_EXPERIENCE += num;
                    user.Update();
                }
            }
            //}
            //else
            //{
            //    oldNum = Convert.ToInt32(user.QQUSER_EXPERIENCE);
            //    if (DateTime.Now.Day - user.UPDATETIME.Day <= 0)
            //    {
            //        message = $"@{qqid} 你今天已经签到过了，请不要重复签到！";
            //    }
            //    else
            //    {
            //        AccessTableHelper.ExecuteNonQuery($"UPDATE QQUSER SET EXPERIENCE = {oldNum + num} WHERE QQID = {qqid} AND QQQID = {qqqid}");
            //    }
            //}

            //if (string.IsNullOrWhiteSpace(message))
            //    message = $"@{qqid} 签到成功，当前经验值为：{oldNum}(+{num})={oldNum + num}";
            return message;
        }
        public string HttpGet(string Url, string postDataStr)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.CookieContainer = new CookieContainer();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.Default);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
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