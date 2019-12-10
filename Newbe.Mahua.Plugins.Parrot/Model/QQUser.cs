using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model.Base;

namespace Newbe.Mahua.Plugins.Parrot.Model
{
    /// <summary>
    /// QQ用户表
    /// </summary>
    [Table(Name = nameof(QQUSER))]
    public class QQUSER : BaseEntity
    {
        public enum CurrentAddress
        {
            DQ = 0

        }
        public enum STATE
        {
            /// <summary>
            /// 没有状态
            /// </summary>
            NOSTATE = 0,
            STATE = 1
        }
        /// <summary>
        /// QQ号
        /// </summary>
        [Column(CanBeNull = false, Describe = "QQ号")]
        public long QQUSER_QQID { get; set; }

        /// <summary>
        /// QQ群号
        /// </summary>
        [Column(CanBeNull = false, Describe = "QQ群号")]
        public long QQUSER_QQQID { get; set; }

        /// <summary>
        /// 经验
        /// </summary>
        [Column(CanBeNull = false, Describe = "经验")]
        public long QQUSER_EXPERIENCE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(CanBeNull = false, Describe = "用户状态", Type = ColumnType.INT)]
        public STATE QQUSER_STATE { get; set; }
    }
}
