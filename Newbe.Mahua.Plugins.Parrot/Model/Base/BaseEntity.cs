using Newbe.Mahua.Plugins.Parrot.Helper;
using System;

namespace Newbe.Mahua.Plugins.Parrot.Model.Base
{
    public abstract class BaseEntity : IKey<Guid>
    {
        [Column(CanBeNull = false, Describe = "ID", IsPrimaryKey = true)]
        public virtual Guid ID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(CanBeNull = false, Describe = "创建时间")]
        public DateTime CREATETIME { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column(CanBeNull = false, Describe = "更新时间")]
        public DateTime UPDATETIME { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column(Describe = "备注")]
        public string BREAK { get; set; }
    }
}
