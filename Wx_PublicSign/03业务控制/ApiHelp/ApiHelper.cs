using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;

namespace ApiHelp
{
    /// <summary>
    /// 一些帮助函数
    /// </summary>
    public class ApiHelper
    {
        #region Json 转换操作

        /// <summary>
        /// 对象转为json字符串
        /// </summary>
        /// <param name="o">json对象</param>
        /// <returns></returns>
        public static string JsonSerializeToStr(object o)
        {
            StringBuilder sb = new StringBuilder();
            JavaScriptSerializer json = new JavaScriptSerializer();
            json.Serialize(o, sb);
            return sb.ToString();
        }

        /// <summary>
        /// json字符串转为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns></returns>
        public static T JsonDeserializeToObj<T>(string jsonString)
        {
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                return (T)ser.ReadObject(ms);
            }
        }

        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }

        #endregion

        #region 加密算法

        #region 不可逆加密
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="oldStr">原字符串</param>
        /// <param name="type">加密方式(SHA1-MD5)</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string oldStr, string type = "SHA1")
        {
            if (!type.Equals("SHA1") && !type.Equals("MD5"))
            {
                return string.Empty;
            }
            return FormsAuthentication.HashPasswordForStoringInConfigFile(oldStr, type); ;
        }
        #endregion

        #region 可逆加密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="oldStr">原字符串</param>
        /// <returns></returns>
        public static string InverseEncode(string oldStr)
        {
            char[] charKey ={
                '+','-','*','/','4','5','6','7'
                };
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(charKey);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(charKey);
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cst);
            sw.Write(oldStr);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="oldStr">源字符串</param>
        /// <returns></returns>
        public static string InverseDecode(string oldStr)
        {
            char[] charKey ={
                '+','-','*','/','4','5','6','7'
                };
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(charKey);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(charKey);
            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(oldStr);
            }
            catch
            {
                return null;
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }


        #endregion

        #endregion

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="Length">数字长度</param>
        /// <param name="charLength">获取生成字符所包含数组长度</param>
        /// <param name="firstStr">头部包含</param>
        /// <returns></returns>
        public static string CreateRandom(int Length, int charLength, string firstStr)
        {
            //生成字符串所包含
            char[] constant ={
                '0','1','2','3','4','5','6','7','8','9',
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
                };
            StringBuilder newRandom = new StringBuilder();
            newRandom.Append(firstStr);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(charLength)]);
            }
            return newRandom.ToString();
        }

        /// <summary>
        /// 计算2个点(经纬度)之间的直线距离
        /// </summary>
        /// <param name="lat1">纬度1</param>
        /// <param name="lng1">经度1</param>
        /// <param name="lat2">纬度2</param>
        /// <param name="lng2">经度2</param>
        /// <param name="way">计算距离方式</param>
        /// <returns></returns>
        public static double DistanceFromJW(double lat1, double lng1, double lat2, double lng2, bool way = true)
        {
            double EARTH_RADIUS = 6378.137;//地球半径
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double radLon1 = rad(lng1);
            double radLon2 = rad(lng2);

            if (way)
            {
                if (radLat1 < 0) radLat1 = Math.PI / 2 + Math.Abs(radLat1);// south   
                if (radLat1 > 0) radLat1 = Math.PI / 2 - Math.Abs(radLat1);// north  
                if (radLon1 < 0) radLon1 = Math.PI * 2 - Math.Abs(radLon1);// west  
                if (radLat2 < 0) radLat2 = Math.PI / 2 + Math.Abs(radLat2);// south  
                if (radLat2 > 0) radLat2 = Math.PI / 2 - Math.Abs(radLat2);// north  
                if (radLon2 < 0) radLon2 = Math.PI * 2 - Math.Abs(radLon2);// west  
                double x1 = EARTH_RADIUS * Math.Cos(radLon1) * Math.Sin(radLat1);
                double y1 = EARTH_RADIUS * Math.Sin(radLon1) * Math.Sin(radLat1);
                double z1 = EARTH_RADIUS * Math.Cos(radLat1);
                double x2 = EARTH_RADIUS * Math.Cos(radLon2) * Math.Sin(radLat2);
                double y2 = EARTH_RADIUS * Math.Sin(radLon2) * Math.Sin(radLat2);
                double z2 = EARTH_RADIUS * Math.Cos(radLat2);
                double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
                //余弦定理求夹角  
                double theta = Math.Acos((EARTH_RADIUS * EARTH_RADIUS + EARTH_RADIUS * EARTH_RADIUS - d * d) / (2 * EARTH_RADIUS * EARTH_RADIUS));
                double dist = theta * EARTH_RADIUS;
                return dist;
            }
            else
            {
                double a = radLat1 - radLat2;
                double b = rad(lng1) - rad(lng2);
                double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                 Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
                s = s * EARTH_RADIUS;
                s = Math.Round(s * 10000) / 10000;
                return s;
            }
        }

        /// <summary>
        /// 计算坐标
        /// </summary>
        /// <param name="d">地图坐标</param>
        /// <returns></returns>
        public static double rad(double d) { return d * Math.PI / 180.0; }

        /// <summary>
        /// 邮件发送代理
        /// </summary>
        /// <param name="MTo">发送人邮箱(可以多个)</param>
        /// <param name="Subject">邮件标题</param>
        /// <param name="Body">邮件内容(可为html)</param>
        /// <param name="file">发送附件地址(文件绝对路径)</param>
        /// <returns></returns>
        public static bool MailSend(string[] MTo, string Subject, string Body, string[] file = null)
        {
            try
            {
                SmtpClient sc = new SmtpClient();
                sc.Host = "smtp.qq.com";//smtp服务器地址
                sc.Port = 25;//默认端口为25 
                sc.UseDefaultCredentials = true;
                sc.EnableSsl = true;
                //发送人账号、授权密码
                sc.Credentials = new System.Net.NetworkCredential("1608995730@qq.com", "psnztvmrvzeibaei");
                MailAddress mf = new MailAddress("1608995730@qq.com", "hl", Encoding.GetEncoding("utf-8"));
                MailMessage message = new MailMessage();
                message.From = mf;
                message.Subject = Subject;//标题
                message.Body = Body;//内容
                message.IsBodyHtml = true;           //是否为html格式 
                message.Priority = MailPriority.High;  //发送邮件的优先等级 
                //多个发送人
                for (int i = 0; i < MTo.Length; i++)
                {
                    message.To.Add(MTo[i]);
                }

                //发送附件
                if (file != null)
                {
                    for (int i = 0; i < file.Length; i++)
                    {
                        message.Attachments.Add(new Attachment(file[i]));
                    }
                }

                sc.Send(message);
            }//抛出异常
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimgLongUnix()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
            return timeStamp.ToString();
        }

        /// <summary>
        /// 解析JSON对象键值队
        /// </summary>
        /// <param name="obj">字典对象</param>
        /// <param name="key">key</param>
        /// <param name="defaultValue">value</param>
        /// <returns></returns>
        public static string GetProertyValue(JObject obj, string key, string defaultValue)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            var property = obj[key];
            if (property != null && !string.IsNullOrEmpty(property.ToString()))
            {
                return property.ToString();
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取当前站点地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebLocation()
        {
            string s = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.RawUrl, "");
            return s.Replace("https://", "").Replace("http://", "");
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }
    }
}
