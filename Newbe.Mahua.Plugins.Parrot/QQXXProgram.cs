using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;
using System;

namespace Newbe.Mahua.Plugins.Parrot
{
    class QQXXProgram
    {
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


        public QQUSER.STATE GetQQXXUserState()
        {
            QQUSER.STATE state = QQUSER.STATE.NOSTATE;
            var res = EntityHelper.Get<QQUSER>(p => p.QQUSER_QQID == QQID && p.QQUSER_QQQID == QQQID);
            if (res != null)
            {
                state = res.QQUSER_STATE;
            }
            return state;
        }

        public bool QQXXLogin()
        {
            try
            {
                if (GetQQXXUserState() != QQUSER.STATE.NOSTATE)
                {
                    var res = EntityHelper.Get<QQUSER>(p => p.QQUSER_QQID == QQID && p.QQUSER_QQQID == QQQID);
                    res.QQUSER_STATE = QQUSER.STATE.STATE;
                    res.Update();
                }
                return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }
    }
}
