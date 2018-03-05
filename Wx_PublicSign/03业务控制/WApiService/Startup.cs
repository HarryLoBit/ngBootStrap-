using Microsoft.Owin;
using Owin;
using System.Web.Http;
[assembly: OwinStartup(typeof(WApiService.Startup))]
namespace WApiService
{
    public partial class Startup
    {
        public static HttpConfiguration HttpConfiguration { get; private set; }
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
