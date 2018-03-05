using Chloe.Entity;
using System;
namespace SugarModel
{
    /// <summary>
    /// 模型基类
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 逻辑主键
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
