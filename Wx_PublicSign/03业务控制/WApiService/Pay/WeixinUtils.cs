using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using WxPayAPI;

public class WeixinUtils
{
    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="encode"></param>
    /// <returns></returns>
    public static string CreateGetHttpResult(string url, Encoding encode)
    {
        var request = WebRequest.Create(url) as HttpWebRequest;
        request.Method = "GET";
        var response = request.GetResponse() as HttpWebResponse;
        using (var sr = new StreamReader(response.GetResponseStream(), encode))
        {
            var result = sr.ReadToEnd();
            return result;
        }
    }

    /// <summary>
    /// 创建Post方式的Http请求结果
    /// </summary>
    /// <param name="url">http地址</param>
    /// <param name="data">提交的数据json</param>
    /// <param name="encode">编码方式</param>
    /// <returns></returns>
    public static string CreatePostHttpResult(string url, string data, Encoding encode)
    {
        var request = WebRequest.Create(url) as HttpWebRequest;
        request.Method = "POST";
        request.ContentType = "application/x-www-from-urlencoded";
        var bytes = encode.GetBytes(data);
        request.ContentLength = bytes.Length;
        using (var stream = request.GetRequestStream())
        {
            stream.Write(bytes, 0, bytes.Length);
        }
        var response = request.GetResponse() as HttpWebResponse;
        using (var sr = new StreamReader(response.GetResponseStream(), encode))
        {
            var result = sr.ReadToEnd();
            return result;
        }
    }

    /// <summary>
    /// 获取String1
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    public static string GetString1(Dictionary<string, string> dic)
    {
        string string1 = string.Empty;
        dic = dic.OrderBy(z => z.Key).ToDictionary(z => z.Key, z => z.Value);
        foreach (var kvp in dic)
        {
            string1 += string.Format("{0}={1}&", kvp.Key, kvp.Value);
        }
        string1 = string1.Substring(0, string1.Length - 1);
        return string1;
    }

    /// <summary>  
    /// 获取当前时间戳  
    /// </summary>  
    /// <param name="UtcDateTime">Utc时间</param>  
    /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>  
    /// <returns></returns>  
    public static string GetTimeStamp(DateTime UtcDateTime, bool bflag = true)
    {
        TimeSpan ts = UtcDateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        string ret = string.Empty;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds).ToString();
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();

        return ret;
    }

    /// <summary>
    /// 获取guid
    /// </summary>
    /// <returns></returns>
    public static string GetGuid()
    {
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string MD5(string str)
    {
        return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToUpper();
    }

    /// <summary>
    /// 把流转为byte数组
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static byte[] StreamToBytes(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        // 设置当前流的位置为流的开始
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }

    /// <summary>
    /// 添加数据缓存，若存在则移除重新添加
    /// </summary>
    /// <param name="key">指定的键</param>
    /// <param name="value">需要设置的值</param>
    /// <param name="absoluteExpiration">到期时间并被移除的时间</param>
    /// <param name="slidingExpiration">插入时间与到期时间的间隔</param>
    public static void SetCache(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
    {
        var cache = HttpRuntime.Cache;
        if (cache[key] != null)
        {
            cache.Remove(key);
        }
        cache.Insert(key, value, null, absoluteExpiration, slidingExpiration);
    }

    /// <summary>
    /// 获取数据缓存
    /// </summary>
    /// <param name="key">指定的键</param>
    /// <param name="defaultValue">缺省值</param>
    /// <returns>指定的数据缓存项</returns>
    public static object GetCache(string key, object defaultValue = null)
    {
        var cache = HttpRuntime.Cache;
        if (cache[key] == null)
        {
            return defaultValue;
        }
        else
        {
            return cache[key];
        }
    }

    /// <summary>
    /// 获取access_token
    /// </summary>
    /// <returns></returns>
    public static string GetAccessToken()
    {
        var cache = GetCache("WXACCESSTOKEN", null);
        if (cache == null)
        {
            string appid = WxPayConfig.APPID; 
            string secret = WxPayConfig.APPSECRET;
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
            string result = CreateGetHttpResult(url, Encoding.UTF8);
            accesstokeninfo _accesstokeninfo = new JavaScriptSerializer().Deserialize<accesstokeninfo>(result);
            DateTime absoluteExpiration = DateTime.MaxValue;
            SetCache("WXACCESSTOKEN", _accesstokeninfo.access_token, absoluteExpiration, TimeSpan.FromSeconds(7200));
            return _accesstokeninfo.access_token;
        }
        else
        {
            return cache.ToString();
        }
    }

    /// <summary>
    /// 获取jsapi_ticket
    /// </summary>
    /// <returns></returns>
    public static string GetJsapiTicket()
    {
        var cache = GetCache("WXTICKET", null);
        if (cache == null)
        {
            var access_token = GetAccessToken();
            string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + access_token + "&type=jsapi";
            string result = CreateGetHttpResult(url, Encoding.UTF8);
            ticketinfo _ticketinfo = new JavaScriptSerializer().Deserialize<ticketinfo>(result);
            DateTime absoluteExpiration = DateTime.MaxValue;
            SetCache("WXTICKET", _ticketinfo.ticket, absoluteExpiration, TimeSpan.FromSeconds(7200));
            return _ticketinfo.ticket;
        }
        else
        {
            return cache.ToString();
        }
    }

    public static string Unifiedorder(string device_info, string nonce_str, string body, string detail, string attach, string out_trade_no, string fee_type, string total_fee, string spbill_create_ip, string time_start, string time_expire, string goods_tag, string notify_url, string trade_type, string product_id, string limit_pay, string openid, out string ErrorMsg)
    {
        var Dic = new Dictionary<string, string>();
        Dic.Add("appid", WxPayConfig.APPID);
        Dic.Add("mch_id", WxPayConfig.MCHID);
        if (!string.IsNullOrEmpty(device_info))
        {
            Dic.Add("device_info", device_info);
        }
        if (!string.IsNullOrEmpty(nonce_str))
        {
            Dic.Add("nonce_str", nonce_str);
        }
        if (!string.IsNullOrEmpty(body))
        {
            Dic.Add("body", body);
        }
        if (!string.IsNullOrEmpty(detail))
        {
            Dic.Add("detail", detail);
        }
        if (!string.IsNullOrEmpty(attach))
        {
            Dic.Add("attach", attach);
        }
        if (!string.IsNullOrEmpty(out_trade_no))
        {
            Dic.Add("out_trade_no", out_trade_no);
        }
        if (!string.IsNullOrEmpty(fee_type))
        {
            Dic.Add("fee_type", fee_type);
        }
        if (!string.IsNullOrEmpty(total_fee))
        {
            Dic.Add("total_fee", total_fee);
        }
        if (!string.IsNullOrEmpty(spbill_create_ip))
        {
            Dic.Add("spbill_create_ip", spbill_create_ip);
        }
        if (!string.IsNullOrEmpty(time_start))
        {
            Dic.Add("time_start", time_start);
        }
        if (!string.IsNullOrEmpty(time_expire))
        {
            Dic.Add("time_expire", time_expire);
        }
        if (!string.IsNullOrEmpty(goods_tag))
        {
            Dic.Add("goods_tag", goods_tag);
        }
        if (!string.IsNullOrEmpty(notify_url))
        {
            Dic.Add("notify_url", notify_url);
        }
        if (!string.IsNullOrEmpty(trade_type))
        {
            Dic.Add("trade_type", trade_type);
        }
        if (!string.IsNullOrEmpty(product_id))
        {
            Dic.Add("product_id", product_id);
        }
        if (!string.IsNullOrEmpty(limit_pay))
        {
            Dic.Add("limit_pay", limit_pay);
        }
        if (!string.IsNullOrEmpty(openid))
        {
            Dic.Add("openid", openid);
        }
        var string1 = GetString1(Dic);
        string key = WxPayConfig.KEY;
        string1 += string.Format("&key={0}", key);
        //MD5加密
        var sign = MD5(string1);
        Dic.Add("sign", sign);
        string package = "<xml>";
        foreach (KeyValuePair<string, string> kvp in Dic)
        {
            if (kvp.Value.All(c => char.IsDigit(c)))
            {
                package += string.Format("<{0}>{1}</{0}>", kvp.Key, kvp.Value);
            }
            else
            {
                package += string.Format("<{0}><![CDATA[{1}]]></{0}>", kvp.Key, kvp.Value);
            }
        }
        package += "</xml>";
        string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
        string result = CreatePostHttpResult(url, package, Encoding.UTF8);
        XmlDocument xd = new XmlDocument();
        xd.LoadXml(result);
        string return_code = xd.SelectSingleNode("xml/return_code").InnerText;
        ErrorMsg = string.Empty;
        if (return_code == "SUCCESS")
        {
            string result_code = xd.SelectSingleNode("xml/result_code").InnerText;
            if (result_code == "SUCCESS")
            {
                string prepay_id = xd.SelectSingleNode("xml/prepay_id").InnerText;
                return prepay_id;
            }
            else
            {
                string err_code = xd.SelectSingleNode("xml/err_code").InnerText;
                string err_code_des = xd.SelectSingleNode("xml/err_code_des").InnerText;
                ErrorMsg = err_code_des;
                return "";
            }
        }
        else
        {
            string return_msg = xd.SelectSingleNode("xml/return_msg").InnerText;
            ErrorMsg = return_msg;
            return "";
        }

    }

    public class accesstokeninfo
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
    public class ticketinfo
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public int expires_in { get; set; }
    }
}

