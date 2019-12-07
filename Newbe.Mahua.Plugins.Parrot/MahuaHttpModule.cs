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
            //cd ����
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
            //������Ϣ
            if (body != null && body.TypeCode == "ProcessPrivateMessage" && body.Platform == 0)
            {
                switch (body.Msg)
                {
                    case "���":
                        body.Msg = "��Ҳ��ѽ";
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
            //Ⱥ��Ϣ
            else if (body != null && body.TypeCode == "ProcessGroupMessage" && body.Platform == 0)
            {
                body.Message = StartProcessing(body.FromQQ, body.FromGroup, body.Message.Trim());
                if (!string.IsNullOrWhiteSpace(body.Message))
                    cqpApi.Apiv1CqpCQSendGroupMsg(new CqpCQSendGroupMsgHttpInput
                    {
                        Msg = body.Message,
                        Ⱥ�� = body.FromGroup
                    });
                message = "ok";
            }
            return message == string.Empty ? "nothing" : message;
        }

        /// <summary>
        /// ��ʼ�ӹ�����
        /// </summary>
        /// <param name="qqid">qq��</param>
        /// <param name="qqqid">qqȺ��</param>
        /// <param name="message">��Ϣ</param>
        /// <returns></returns>
        public string StartProcessing(long qqid, long qqqid, string message)
        {
            if (message == "�ճ�")
            {
                message = JX3RCSearch();
            }
            if (message.Contains("����"))
            {
                message = message.Split(new char[2] { '��', '��' })[message.Split(new char[2] { '��', '��' }).Length - 1];
                message = JX3KFSearch(message.Trim());
            }
            else if (message == "ǩ��")
            {
                message = QQQQD(qqid, qqqid);
            }
            else if (message == "����")
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
        /// �������ճ���ѯ
        /// </summary>
        /// <returns></returns>
        public string JX3RCSearch()
        {
            return string.Empty;
        }

        /// <summary>
        /// ������������ѯ
        /// </summary>
        /// <returns>���ز�ѯ���</returns>
        public string JX3KFSearch(string serverName)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            {
                return "��������������ƣ���ѯ��ʽ�ǣ�\"�����ν���\"";
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
                return "û���������������ѯ��ʽ�ǣ�\"�����ν���\"";
            }
            switch (resCode)
            {
                case 8:
                    serverName += "�ѿ�������ǰ������״̬����";
                    break;
                case 7:
                    serverName += "�ѿ�������ǰ������״̬��ӵ��";
                    break;
                case 6:
                    serverName += "�ѿ�������ǰ������״̬������";
                    break;
                default:
                    serverName += "δ������";
                    break;
            }
            return serverName;
        }

        /// <summary>
        /// �ַ�������ת��
        /// </summary>
        /// <param name="srcEncoding">ԭ����</param>
        /// <param name="dstEncoding">Ŀ�����</param>
        /// <param name="srcBytes">ԭ�ַ���</param>
        /// <returns>�ַ���</returns>
        public string TransferEncoding(Encoding srcEncoding, Encoding dstEncoding, string srcStr)
        {
            byte[] srcBytes = srcEncoding.GetBytes(srcStr);
            byte[] bytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(bytes);
        }

        /// <summary>
        /// QQȺǩ��
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
                //���������
                user.QQUSER_QQID = qqid;
                user.QQUSER_QQQID = qqqid;
                user.QQUSER_EXPERIENCE = num;
                user.Insert();
            }
            else
            {
                if (DateTime.Now.Day == user.UPDATETIME.Day)
                {
                    message = string.Format("@{0} ������Ѿ�ǩ�����ˣ��벻Ҫ�ظ�ǩ����", qqid);
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
            //        message = $"@{qqid} ������Ѿ�ǩ�����ˣ��벻Ҫ�ظ�ǩ����";
            //    }
            //    else
            //    {
            //        AccessTableHelper.ExecuteNonQuery($"UPDATE QQUSER SET EXPERIENCE = {oldNum + num} WHERE QQID = {qqid} AND QQQID = {qqqid}");
            //    }
            //}

            //if (string.IsNullOrWhiteSpace(message))
            //    message = $"@{qqid} ǩ���ɹ�����ǰ����ֵΪ��{oldNum}(+{num})={oldNum + num}";
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
                Console.WriteLine("��վ����{0}", ex.Message);
            }

            return "��վ����";
        }

    }

    public class BodyModel
    {
        /// <summary>
        /// QQ��
        /// </summary>
        public long FromQQ { get; set; }
        /// <summary>
        /// ˽����Ϣ
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// QQȺ��
        /// </summary>
        public long FromGroup { get; set; }
        /// <summary>
        /// Ⱥ����Ϣ
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