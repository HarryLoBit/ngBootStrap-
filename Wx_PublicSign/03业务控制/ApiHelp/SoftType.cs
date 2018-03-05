using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    /// <summary>
    /// TryParse软类型转换
    /// 2017-06-20 15:11:04 郝亮
    /// </summary>
    public static class SoftType
    {

        /// <summary>
        /// 转为int类型
        /// </summary>
        /// <param name="obj">要转化的obj</param>
        /// <param name="defval">默认值</param>
        /// <returns></returns>
        public static int _Int(this object obj, int defval = 0)
        {
            int a = 0;
            decimal b = decimal.Zero;
            decimal.TryParse(obj.ToString(), out b);
            if (b == 0) { a = defval; }
            else { a = Convert.ToInt32(b); }
            return a;
        }

        /// <summary>
        /// 转为string类型
        /// </summary>
        /// <param name="obj">要转化的obj</param>
        /// <param name="defval">默认值</param>
        /// <returns></returns>
        public static string _String(this object obj, string defval = "")
        {
            string a = defval;
            if (obj == null) { return a; }
            a = obj.ToString();
            return a;
        }

        /// <summary>
        /// 转为Bool类型
        /// </summary>
        /// <param name="obj">要转化的obj</param>
        /// <param name="defval">默认值</param>
        /// <returns></returns>
        public static bool _Bool(this object obj, bool defval = false)
        {
            bool a = false;
            if (!bool.TryParse(obj.ToString(), out a)) { a = defval; };
            return a;
        }

        /// <summary>
        /// 转为DateTime类型
        /// </summary>
        /// <param name="obj">要转化的obj</param>
        /// <returns></returns>
        public static DateTime _DateTime(this object obj)
        {
            DateTime a = DateTime.Parse("1900-01-01");
            DateTime.TryParse(obj.ToString(), out a);
            return a;
        }

        /// <summary>
        /// 转为DateTime类型2
        /// </summary>
        /// <param name="obj">要转化的obj</param>
        /// <returns></returns>
        public static DateTime? _DateTimeToNull(this object obj)
        {
            DateTime a = DateTime.Parse("1900-01-01");
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return null;
            }
            DateTime.TryParse(obj.ToString(), out a);
            return a;
        }

        /// <summary>
        /// 转为decimal类型
        /// </summary>
        /// <param name="obj">要转化的obj</param>
        /// <param name="number">保留小数的位数</param>
        /// <param name="defval">默认值</param>
        /// <returns></returns>
        public static decimal _Decimal(this object obj, int number = 2, decimal defval = decimal.Zero)
        {
            decimal a = decimal.Zero;
            if (!decimal.TryParse(obj.ToString(), out a)) { a = defval; };
            a = Math.Round(a, number);//四舍五入保留两位小数
            return a;
        }

        /// <summary>
        /// 转为double类型
        /// </summary>
        /// <param name="obj">要转化的obj</param>
        /// <param name="defval">默认值</param>
        /// <returns></returns>
        public static double _Double(this object obj, double defval = 0)
        {
            double a = 0;
            if (!double.TryParse(obj.ToString(), out a)) { a = defval; };
            return a;
        }

        /// <summary>
        /// 转为float类型
        /// </summary>
        /// <param name="obj">要转化的obj</param>
        /// <param name="defval">默认值</param>
        /// <returns></returns>
        public static double _Float(this object obj, float defval = 0)
        {
            float a = 0;
            if (!float.TryParse(obj.ToString(), out a)) { a = defval; };
            return a;
        }
    }
}