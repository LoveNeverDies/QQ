using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;

namespace Newbe.Mahua.Plugins.Parrot
{
    class QQXXProgram
    {
        public QQXXProgram()
        {

        }

        public QQUSER.STATE GetQQXXUserState(long qqid, long qqqid)
        {
            QQUSER.STATE state = QQUSER.STATE.NOSTATE;
            var res = EntityHelper.Get<QQUSER>(p => p.QQUSER_QQID == qqid && p.QQUSER_QQQID == qqqid);
            if (res != null)
            {
                state = res.QQUSER_STATE;
            }
            return state;
        }

        public bool QQXXLogin()
        {
            return true;
        }
    }
}
