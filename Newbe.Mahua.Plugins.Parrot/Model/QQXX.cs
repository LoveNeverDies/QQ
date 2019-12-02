using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model.Base;
using System.Collections.Generic;

namespace Newbe.Mahua.Plugins.Parrot.Model
{
    /// <summary>
    /// QQ修仙表
    /// </summary>
    [Table(Name = nameof(QQXX))]
    public class QQXX : BaseEntity
    {
        /// <summary>
        /// 关联QQUSER表
        /// </summary>
        [Column(Type = ColumnType.UNIQUEIDENTIFIER, Describe = "关联QQUSER表", IsForeignKey = true)]
        public List<QQUSER> QQUSER_GUID { get; set; }

        /// <summary>
        /// 关联QQXXLEVEL表
        /// </summary>
        [Column(Type = ColumnType.UNIQUEIDENTIFIER, Describe = "关联QQXXLEVEL表", IsForeignKey = true)]
        public List<QQXXLEVEL> QQXXLEVEL_GUID { get; set; }

        /// <summary>
        /// 关联QQXXZM表
        /// </summary>
        [Column(Type = ColumnType.UNIQUEIDENTIFIER, Describe = "关联QQXXZM表", IsForeignKey = true)]
        public List<QQXXZM> QQXXZM_GUID { get; set; }

        /// <summary>
        /// 真气（攻击）
        /// </summary>
        public long QQXX_ZQ { get; set; }

        /// <summary>
        /// 体魄（生命）
        /// </summary>
        public long QQXX_TP { get; set; }

        /// <summary>
        /// 根骨（防御）
        /// </summary>
        public long QQXX_GG { get; set; }

        /// <summary>
        /// 身法（闪避）
        /// </summary>
        public long QQXX_SF { get; set; }

        /// <summary>
        /// 悟性（修炼速度）
        /// </summary>
        public long QQXX_WX { get; set; }

        /// <summary>
        /// 修炼年限
        /// </summary>
        public long QQXX_YEAR { get; set; }

        /// <summary>
        /// 灵石
        /// </summary>
        public long QQXX_LS { get; set; }

        /// <summary>
        /// 威望
        /// </summary>
        public long QQXX_WW { get; set; }

        /// <summary>
        /// 宗门贡献
        /// </summary>
        public long QQXX_ZMGX { get; set; }
    }
}
