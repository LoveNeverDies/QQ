using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model.Base;

namespace Newbe.Mahua.Plugins.Parrot.Model
{
    /// <summary>
    /// QQ修仙门派
    /// </summary>
    [Table(Name = nameof(QQXXMP))]
    public class QQXXMP : BaseEntity
    {
        /// <summary>
        /// 门派名称
        /// </summary>
        [Column(CanBeNull = false, Type = ColumnType.NVARCHAR100, Describe = "门派名称")]
        public string QQXXMP_NAME { get; set; }

        /// <summary>
        /// 加入门派需要的境界
        /// </summary>
        [Column(CanBeNull = false, Type = ColumnType.NVARCHAR100, Describe = "加入门派需要的境界")]
        public string QQXXMP_LEVEL { get; set; }

        /// <summary>
        /// 门派的好坏true是好false是坏
        /// </summary>
        [Column(CanBeNull = false, Describe = "门派的好坏true是好false是坏")]
        public bool QQXXMP_HH { get; set; }
        /// <summary>
        /// 门派介绍
        /// </summary>
        [Column(CanBeNull = false, Describe = "门派介绍")]
        public string QQXXMP_JS { get; set; }
    }
}
