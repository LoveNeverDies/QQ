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
            DQ = 0,
            TJ = 1,
            JTZS = 2
        }
        public enum State
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
        /// QQ昵称
        /// </summary>
        [Column(CanBeNull = false, Describe = "QQ昵称")]
        public string QQUSER_QQNAME { get; set; }

        /// <summary>
        /// QQ群号
        /// </summary>
        [Column(CanBeNull = false, Describe = "QQ群号")]
        public long QQUSER_QQQID { get; set; }

        /// <summary>
        /// QQ群昵称
        /// </summary>
        [Column(CanBeNull = false, Describe = "QQ群昵称")]
        public string QQUSER_QQQNAME { get; set; }

        /// <summary>
        /// 经验
        /// </summary>
        [Column(CanBeNull = false, Describe = "经验")]
        public long QQUSER_EXPERIENCE { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        [Column(CanBeNull = false, Describe = "用户状态", Type = ColumnType.INT)]
        public State QQUSER_STATE { get; set; }

        /// <summary>
        /// 角色所在位置
        /// </summary>
        [Column(CanBeNull = false, Describe = "角色所在位置", Type = ColumnType.INT)]
        public CurrentAddress QQUSER_CURRENTADDRESS { get; set; }
    }
}
