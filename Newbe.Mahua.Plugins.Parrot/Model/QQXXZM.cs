using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model.Base;

namespace Newbe.Mahua.Plugins.Parrot.Model
{
    /// <summary>
    /// QQ修仙宗门
    /// </summary>
    [Table(Name = nameof(QQXXZM))]
    public class QQXXZM : BaseEntity
    {
        /// <summary>
        /// 宗门名称
        /// </summary>
        [Column(CanBeNull = false, Describe = "宗门名称")]
        public string QQXXZM_NAME { get; set; }

        /// <summary>
        /// 加入宗门需要的境界
        /// </summary>
        [Column(CanBeNull = false, Describe = "加入宗门需要的境界")]
        public string QQXXZM_LEVEL { get; set; }

        /// <summary>
        /// 宗门的好坏true是好false是坏
        /// </summary>
        [Column(CanBeNull = false, Describe = "宗门的好坏true是好false是坏")]
        public bool QQXXZM_HH { get; set; }
    }
}
