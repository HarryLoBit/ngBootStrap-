using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Core
{
    /// <summary>
    /// 转换从 DataReader 读出来的数据库原始值
    /// </summary>
    public interface IDbValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerValue">调用 IDataReader.GetValue() 方法读出来的原始值，有可能为 DBNull.Value。</param>
        /// <returns>与实体属性类型相同的值。如果 readerValue 为 DBNull.Value，可返回 null。</returns>
        object Convert(object readerValue);
    }
}
