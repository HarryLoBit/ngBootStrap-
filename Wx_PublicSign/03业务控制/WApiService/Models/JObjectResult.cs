using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace WApiService
{
    /// <summary>
    /// 返回值
    /// </summary>
    public class JObjectResult
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 错误信息提示
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public object data { get; set; }
    }
}