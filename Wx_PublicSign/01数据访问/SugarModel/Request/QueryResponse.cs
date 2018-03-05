using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarModel.Request
{
    public class QueryResponse<T>
    {
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }

    }
}
