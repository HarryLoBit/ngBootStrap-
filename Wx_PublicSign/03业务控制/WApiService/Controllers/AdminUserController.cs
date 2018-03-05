using ApiHelp;
using BossWellApp;
using SugarModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WApiService.Models.RequestModel;

namespace WApiService.Controllers
{
    public class AdminUserController : ApiController
    {
        AdminUserApp app = new AdminUserApp();
        JObjectResult result = new JObjectResult();
        public AdminUserController()
        {
            result.code = 200;
            result.msg = "ok";
        }

        /// <summary>
        /// list
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/adminuser/list")]
        public async Task<JObjectResult> QueryList(string search)
        {
            result.data = app.GetList(search);
            return result;
        }


        /// <summary>
        /// Single
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/adminuser/single")]
        public async Task<JObjectResult> QuerySingle(string sid)
        {
            result.data = app.GetSingle(sid);
            return result;
        }

        /// <summary>
        /// Insert and Update
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/adminuser/save")]
        public async Task<JObjectResult> InsertAndUpdate(AdminUser Model)
        {
            result.data = app.Save(Model);
            return result;
        }

        /// <summary>
        /// list
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/adminuser/delt")]
        public async Task<JObjectResult> Delete(string sid)
        {
            result.data = app.Delete(sid);
            return result;
        }

    }
}
