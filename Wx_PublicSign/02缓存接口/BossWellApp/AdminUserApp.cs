using BossWellService;
using IBossWellService;
using SugarModel;
using SugarModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BossWellApp
{
    public class AdminUserApp : AdminUserService
    {
        //IAdminUserService app = new AdminUserService();

        public QueryResponse<AdminUser> GetList(string search)
        {
            QueryRequest<AdminUser> request = new QueryRequest<AdminUser>();
            
            request.expression = (t => t.Account.Contains(search) || t.NickName.Contains(search));
            request.Page = 1;
            request.PageSize = 10;
            return QueryBasice<AdminUser>(request);
        }
        public AdminUser GetSingle(string sid)
        {
            return QuerySing<AdminUser>(t => t.Sid == sid);
        }

        public AdminUser Save(AdminUser saveModel)
        {
            return InsertAndUpdate<AdminUser>(saveModel,"adminuser_");
        }

        public int Delete(string sid)
        {
            return Deleteable<AdminUser>(t => t.Sid == sid);
        }

    }
}
