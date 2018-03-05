using Chloe;
using Chloe.MySql;
using SugarModel;
using SugarModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ChloeORM
{
    /// <summary>
    /// 操作数据库封装
    /// </summary>
    public partial class ChloeClient : IDisposable
    {

        /// <summary>
        /// 创建链接对象
        /// </summary>
        private readonly MySqlContext context;
        public ChloeClient()
        {
            if (context == null)
            {
                context = new MySqlContext(new MySqlConnectionFactory(DBHelperConnect.ConnectionString));
            }
        }

        public void Dispose()
        {
            this.context.Dispose();
        }

        #region Query

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">查询条件</param>
        /// <returns></returns>
        public virtual QueryResponse<T> QueryBasice<T>(QueryRequest<T> request)
        {
            //返回数据集
            QueryResponse<T> response = new QueryResponse<T>();
            //筛选条件
            var query = context.Query<T>().Where(request.expression);
            //排序条件
            //if (!string.IsNullOrEmpty(request.Sort))
            //{
            //    if (request.SortTp.Equals("desc"))
            //    {
            //        query = query.OrderByDesc();
            //    }
            //    else
            //    {
            //        query = query.OrderBy(request.Sortexpression);
            //    }
            //}
            //总页码
            response.TotalCount = query.Count();
            //分页集合
            response.Items = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            return response;
        }

        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public virtual T QuerySing<T>(Expression<Func<T, bool>> expression)
        {
            return context.Query<T>().Where(expression).FirstOrDefault();
        }

        #endregion

        #region Query Sql

        /// <summary>
        /// 根据sql语句查询列表集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual QueryResponse<T> QueryBySql<T>(string sql, DbParam[] param)
        {

            //List<User> users = context.SqlQuery<User>("select * from Users where Age > ?age", DbParam.Create("?age", 12)).ToList();

            QueryResponse<T> response = new QueryResponse<T>();

            //Sql查询
            var query = context.SqlQuery<T>(sql, param);

            //返回结果集
            response.TotalCount = query.Count();
            response.Items = query.ToList();
            return response;
        }

        #endregion

        #region Save

        /// <summary>
        /// 保存记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        public virtual T InsertAndUpdate<T>(T saveModel, string tit = "") where T : BaseEntity
        {
            T model;
            if (saveModel.Id < 1 && string.IsNullOrEmpty(saveModel.Sid))
            {
                saveModel.Sid = ApiHelp.ApiHelper.CreateRandom(32, 26, tit);
                saveModel.CreateDate = DateTime.Now;
                model = context.Insert<T>(saveModel);
            }
            else
            {
                context.Update<T>(saveModel);
                model = QuerySing<T>(t => t.Sid == saveModel.Sid);
            }
            return model;
        }

        /// <summary>
        /// 批量修改记录
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="express">条件</param>
        /// <param name="sourceExpress">修改模型值</param>
        /// <returns></returns>
        public virtual int Updateable<T>(Expression<Func<T, bool>> express, Expression<Func<T, T>> sourceExpress)
        {
            return context.Update<T>(express, sourceExpress);
        }
        #endregion

        #region Delete

        public virtual int Deleteable<T>(Expression<Func<T, bool>> expression)
        {
            return context.Delete<T>(expression);
        }

        #endregion
    }
}
