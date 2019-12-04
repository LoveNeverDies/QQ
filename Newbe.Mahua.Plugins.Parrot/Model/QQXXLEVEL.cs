﻿using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model.Base;

namespace Newbe.Mahua.Plugins.Parrot.Model
{
    /// <summary>
    /// QQ修仙境界
    /// </summary>
    public class QQXXLEVEL : BaseEntity
    {
        /// <summary>
        /// 境界阶段 例：大乘期一阶
        /// </summary>
        [Column(CanBeNull = false, Type = ColumnType.NVARCHAR100, Describe = "境界阶段")]
        public string QQXXLEVEL_NAMEJD { get; set; }

        /// <summary>
        /// 境界阶段 数字形式来比较 例如开光期是1 xx期是2 以此类推 大乘期是10 用数字表示大乘期一阶的话就是 10.1 小数1表示一阶 以此类推
        /// </summary>
        [Column(CanBeNull = false, Describe = "境界阶段")]
        public int QQXXLEVEL_NUMBERJD { get; set; }
    }
}
