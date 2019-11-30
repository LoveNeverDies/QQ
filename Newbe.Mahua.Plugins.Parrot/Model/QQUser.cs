using System.Data;
using System.Data.SqlClient;
using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.HelperService;
using Newbe.Mahua.Plugins.Parrot.Model.Base;

namespace Newbe.Mahua.Plugins.Parrot.Model
{
    /// <summary>
    /// QQ用户表
    /// </summary>
    [Table(Name = nameof(QQUSER))]
    public class QQUSER : BaseEntity
    {
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
    }
}
