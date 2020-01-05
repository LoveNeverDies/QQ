using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model.Base;
using System;

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
        public Guid QQUSER_GUID { get; set; }

        /// <summary>
        /// 关联QQXXLEVEL表
        /// </summary>
        [Column(Type = ColumnType.UNIQUEIDENTIFIER, Describe = "关联QQXXLEVEL表", IsForeignKey = true)]
        public Guid QQXXLEVEL_GUID { get; set; }

        /// <summary>
        /// 关联QQXXMP表
        /// </summary>
        [Column(Type = ColumnType.UNIQUEIDENTIFIER, Describe = "关联QQXXMP表", IsForeignKey = true)]
        public Guid QQXXMP_GUID { get; set; }

        /// <summary>
        /// 真气（攻击）
        /// </summary>
        [Column(CanBeNull = false, Describe = "真气（攻击）")]
        public long QQXX_ZQ { get; set; }

        /// <summary>
        /// 体魄（生命）
        /// </summary>
        [Column(CanBeNull = false, Describe = "体魄（生命）")]
        public long QQXX_TP { get; set; }

        /// <summary>
        /// 根骨（防御）
        /// </summary>
        [Column(CanBeNull = false, Describe = "根骨（防御）")]
        public long QQXX_GG { get; set; }

        /// <summary>
        /// 身法（闪避）
        /// </summary>
        [Column(CanBeNull = false, Describe = "身法（闪避）")]
        public long QQXX_SF { get; set; }

        /// <summary>
        /// 悟性（修炼速度）
        /// </summary>
        [Column(CanBeNull = false, Describe = "悟性（修炼速度）")]
        public long QQXX_WX { get; set; }

        /// <summary>
        /// 修炼年限
        /// </summary>
        [Column(CanBeNull = false, Describe = "修炼年限")]
        public long QQXX_YEAR { get; set; }

        /// <summary>
        /// 灵石
        /// </summary>
        [Column(CanBeNull = false, Describe = "灵石")]
        public long QQXX_LS { get; set; }

        /// <summary>
        /// 威望
        /// </summary>
        [Column(CanBeNull = false, Describe = "威望")]
        public long QQXX_WW { get; set; }

        /// <summary>
        /// 门派贡献
        /// </summary>
        [Column(CanBeNull = false, Describe = "门派贡献")]
        public long QQXX_MPGX { get; set; }
    }
}
