using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SugarModel.Request
{
    /// <summary>
    /// 请求分页列表
    /// </summary>
    public class QueryRequest<T>
    {
        public Expression<Func<T, bool>> expression { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public string SortTp { get; set; }

    }
}
