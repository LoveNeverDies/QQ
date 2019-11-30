using System;
using System.IO;

namespace Newbe.Mahua.Plugins.Parrot.Helper
{
    public interface ILogHelper
    {
        void Error(Exception e);
        void Error(string message);
        void Error(string message, params object[] ps);
        void Info(Exception e);
        void Info(string message);
        void Info(string message, params object[] ps);
        void Waring(Exception e);
        void Waring(string message);
        void Waring(string message, params object[] ps);
    }

    class LogHelper : ILogHelper
    {
        IJsonHelper jsonHelper = null;
        string NowDate { get { return DateTime.Now.ToString("yyyy-MM-dd"); } }
        string LogPath { get { return jsonHelper.ReadJsonByString("LogPath"); } }
        string LogPathExist { get { return string.Format("{0}/{1}", LogPath, NowDate); } }

        public LogHelper()
        {
            jsonHelper = new JsonHelper();
        }
        void ILogHelper.Error(Exception e)
        {
            if (File.Exists(LogPathExist))
            {
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //打开文件写入
                //不存在就新建txt文件写入
            }
            else
            {

            }
            Console.WriteLine(e.Message);
        }

        void ILogHelper.Error(string message)
        {
            Console.WriteLine(message);
        }
        void ILogHelper.Error(string message, params object[] ps)
        {
            message = string.Format(message, ps);
        }
        void ILogHelper.Info(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        void ILogHelper.Info(string message)
        {
            Console.WriteLine(message);
        }
        void ILogHelper.Info(string message, params object[] ps)
        {
            message = string.Format(message, ps);
        }
        void ILogHelper.Waring(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        void ILogHelper.Waring(string message)
        {
            Console.WriteLine(message);
        }
        void ILogHelper.Waring(string message, params object[] ps)
        {
            message = string.Format(message, ps);
        }
    }
}
