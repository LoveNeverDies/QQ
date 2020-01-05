using Newbe.Mahua.Plugins.Parrot.Entity;
using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace Newbe.Mahua.Plugins.Parrot
{
    class QQXXProgram
    {
        static int ThreadId = 0;
        public static IList<Logout> logoutList = new List<Logout>();

        public static void UserLogoutThread()
        {
            Thread t = new Thread(UserLogoutStart);
            t.Start();
            ThreadId = t.ManagedThreadId;
        }

        private static void UserLogoutStart()
        {
            while (true)
            {
                //如果超过了十分钟
                foreach (var item in logoutList)
                {
                    if (item.LoginTime >= item.LoginTime.AddMinutes(10))
                    {
                        UserLogout(item);
                        logoutList.Remove(item);
                    }
                }
                //停止一分钟
                Thread.Sleep(60000);
            }
        }

        long QQID = 0;
        long QQQID = 0;
        string MSG = string.Empty;
        public QQXXProgram(long qqid, long qqqid, string msg)
        {
            QQID = qqid;
            QQQID = qqqid;
            MSG = msg;
        }

        public void SetMsg(string message)
        {
            MSG = message;
        }

        /// <summary>
        /// 获取用户的状态
        /// </summary>
        /// <returns></returns>
        public QQUSER.State GetQQXXUserState()
        {
            QQUSER.State state = QQUSER.State.NOSTATE;
            var res = EntityHelper.Get<QQUSER>(p => p.QQUSER_QQID == QQID && p.QQUSER_QQQID == QQQID);
            if (res != null)
            {
                state = res.QQUSER_STATE;
            }
            return state;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="logout"></param>
        /// <returns></returns>
        private static bool UserLogout(Logout logout)
        {
            var res = EntityHelper.Get<QQUSER>(p => p.QQUSER_QQID == logout.QQID && p.QQUSER_QQQID == logout.QQQID);
            if (res != null)
            {
                res.QQUSER_STATE = QQUSER.State.NOSTATE;
                res.Update();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public QQUSER QQXXLogin()
        {
            QQUSER res = null;
            if (GetQQXXUserState() == QQUSER.State.NOSTATE)
            {
                res = EntityHelper.Get<QQUSER>(p => p.QQUSER_QQID == QQID && p.QQUSER_QQQID == QQQID);
                if (res != null)
                {
                    res.QQUSER_STATE = QQUSER.State.STATE;
                    res.Update();
                }
            }
            return res;
        }

        public string GetUserCurrent()
        {

            return string.Empty;
        }
    }
}
