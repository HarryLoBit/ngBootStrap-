using System.Web.Http;
using System.Web.Http.Cors;

namespace WApiService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //跨域请求
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            //config.Formatters.Add(new PlainTextTypeFormatter());
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
        }
    }
}
