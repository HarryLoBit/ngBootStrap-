using ApiHelp;
using BossWellApp;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler;
using System;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using WApiService.Models;

namespace WApiService
{

    /// <summary>
    /// 微信公众号
    /// /api/weixin
    /// </summary>
    public class WeiXinController : ApiController,IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            //app.CreateLogs(new SugarModel.SystemLogs()
            //{ Source = "weixin", Title = "begin", Message = "进入，开始" });
            try
            {
                string signature = context.Request["signature"];
                string timestamp = context.Request["timestamp"];
                string nonce = context.Request["nonce"];
                string echostr = context.Request["echostr"];
                string code = context.Request["code"];

                //app.CreateLogs(new SugarModel.Logs()
                //{ Source = "weixin", Title = "param ", Message = context.Request.ToString() });

                if (context.Request.HttpMethod == "GET")
                {
                    //app.CreateLogs(new SugarModel.Logs()
                    //{ Source = "weixin", Title = "get", Message = "进入回调 get" });

                    if (code != null && code.Length > 0)
                    {
                        string accesstoken = HttpHelper.sendHttpPost("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + WeiXinConfig.AppId + "&secret=" + WeiXinConfig.AppSecret + "&code=" + code + "&grant_type=authorization_code", null, "Get");
                        JObject accesstokenp = JObject.Parse(accesstoken);

                        string userinf = HttpHelper.sendHttpPost("https://api.weixin.qq.com/sns/userinfo?access_token=" + accesstokenp["access_token"] + "&openid=" + accesstokenp["openid"] + "&lang=zh_CN", null, "Get");
                        JObject userinfp = JObject.Parse(userinf);

                        string nickName = userinfp["nickname"].ToString();
                        string openid = userinfp["openid"].ToString();
                        string headimgurl = userinfp["headimgurl"].ToString();
                    }
                    else
                    {
                        if (CheckSignature.Check(signature, timestamp, nonce, WeiXinConfig.WeixinToken))
                        {
                            context.Response.Output.Write(echostr);
                        }
                    }
                }
                else
                {
                    //app.CreateLogs(new SugarModel.Logs()
                    //{ Source = "weixin", Title = "post", Message = "进入回调 post" });

                    if (!CheckSignature.Check(signature, timestamp, nonce, WeiXinConfig.WeixinToken))
                    {
                        //app.CreateLogs(new SugarModel.Logs()
                        //{ Source = "weixin", Title = "500", Message = "参数错误" });

                        context.Response.Output.Write("参数错误");
                    }

                    //拼接Post参数模型
                    var postModel = new PostModel()
                    {
                        Signature = context.Request.QueryString["signature"],
                        Msg_Signature = context.Request.QueryString["msg_signature"],
                        Timestamp = context.Request.QueryString["timestamp"],
                        Nonce = context.Request.QueryString["nonce"],
                        Token = WeiXinConfig.WeixinToken,
                        EncodingAESKey = WeiXinConfig.EncodingAESKey,//根据自己后台的设置保持一致
                        AppId = WeiXinConfig.AppId//根据自己后台的设置保持一致
                    };

                    //app.CreateLogs(new SugarModel.Logs()
                    //{ Source = "weixin", Title = "call", Message = "回调事件执行----前" });

                    //回调事件
                    var maxRecordCount = 10;
                    var messageHandler = new CustomMessageHandler(context.Request.InputStream, postModel, maxRecordCount);

                    //app.CreateLogs(new SugarModel.Logs()
                    //{ Source = "weixin", Title = "call", Message = "回调事件执行----后" });

                    messageHandler.Execute();
                    context.Response.Output.Write(messageHandler.ResponseDocument.ToString());
                }
            }
            catch (Exception ex)
            {
                //app.CreateLogs(new SugarModel.Logs()
                //{ Source = "weixin", Title = "catch", Message = ex.Message });
            }

        }
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

    }
}
