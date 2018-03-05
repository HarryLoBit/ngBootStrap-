using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using ApiHelp;

namespace WApiService
{
    public class AuthorizationAttribute : ActionFilterAttribute
    {
        //签名效验开关
        public bool IsOpen = true;

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            string method = request.Method.Method;
            string token = string.Empty,time = string.Empty;
            if (request.Headers.Contains("Authorization"))
            {
                token = HttpUtility.UrlDecode(request.Headers.GetValues("Authorization").FirstOrDefault());
            }
            if (request.Headers.Contains("time"))
            {
                time = HttpUtility.UrlDecode(request.Headers.GetValues("time").FirstOrDefault());
            }

            //GetToken方法不需要进行签名验证
            if (IsOpen)
            {
                //签名算法是否正确
                if (!HttpResponseExtension.CheckToken(actionContext.Request.RequestUri.ToString(),token))
                {
                    JObject obj = new JObject();
                    obj.Add("code", 504);
                    obj.Add("msg", "签名错误");
                    actionContext.Response = HttpResponseExtension.ReturnError(obj);
                }
                base.OnActionExecuting(actionContext);
                return;
            }

        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }

    }
}