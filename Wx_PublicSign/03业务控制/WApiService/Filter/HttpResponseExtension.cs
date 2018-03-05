using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using ApiHelp;
using BossWellApp;
using SugarModel;

namespace WApiService
{
    public class HttpResponseExtension
    {
        private static AdminUserApp app = new AdminUserApp();
        //注入效验token，返回错误信息
        public static HttpResponseMessage ReturnError(JObject content)
        {
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        /// <summary>
        /// 效验token是否有效
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool CheckToken(string url, string token)
        {
            //当前请求URL+时间戳+nfine
            //string longtime = ApiHelper.GetTimgLongUnix();
            //string sign = ApiHelper.MD5Encrypt((url + longtime + "nfine"), "MD5").ToUpper().ToString();
            token = token.Replace("Basic ", "");

            byte[] byPath = Convert.FromBase64String(token);
            token = ASCIIEncoding.Default.GetString(byPath);
            string[] account = token.Split(':');
            if (account != null && account.Count() > 1)
            {
                //AdminUser entity = app.CheckLogin(account[0], account[1]);
                //if (entity != null && entity.Id > 0)
                //{
                //    return true;
                //}
            }
            return false;
        }
    }
}