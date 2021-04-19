using Zeiot.Model.Base;
using Zeiot.Service.Base.Interface;
using System;
using System.Data;
using System.Linq;

namespace Zeiot.Service.Base.Implement
{
    /// <summary>
    /// 通用创建类
    /// </summary>
    public class CommonRepository : ICommonRepository
    {
        /// <summary>
        /// 获取表列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ResultView GetModel<T>(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var model = db.GetModel(sql, parameters, transaction, commandTimeout);
                if (model != null)
                {
                    return new ResultView { Info = model, Result = 1, ResultMessage = "请求成功" };
                }
                else
                {
                    return new ResultView { Info = null, Result = 0, ResultMessage = "未请求到数据" };
                }
            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "请求列表失败e：" + e.Message }; }
        }
        /// <summary>
        /// 根据主键获取实体对象(只能有一个主键)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultView GetModelById<T>(object id, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var model = db.GetModelById(id, transaction, commandTimeout);
                if (model != null)
                {
                    return new ResultView { Info = model, Result = 1, ResultMessage = "请求成功" };
                }
                else
                {
                    return new ResultView { Info = null, Result = 0, ResultMessage = "未请求到数据" };
                }
            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "请求列表失败e：" + e.Message }; }
        }

        /// <summary>
        /// 获取表列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ResultView GetList<T>(IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var carList = db.GetModelList(null, null, transaction, commandTimeout);
                if (carList != null && carList.Count() > 0)
                {
                    return new ResultView { Info = carList.ToList(), Result = 1, ResultMessage = "请求成功" };
                }

                return new ResultView { Info = null, Result = 0, ResultMessage = "未请求到数据" };
            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "请求车列表失败e：" + e.Message }; }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageNum">页码</param>
        /// <param name="rowsNum">每页行数</param>
        /// <param name="strWhere">where条件</param>
        /// <param name="orderBy">Orde by排序</param>
        /// <param name="parameters">parameters参数</param>
        /// <returns></returns>
        public PagedList<T> GetListPage<T>(int pageNum, int rowsNum, string strWhere, string orderBy, object parameters)
        {
            IBaseRepository<T> db = new BaseRepository<T>();
            var pageList = db.GetListPage(pageNum, rowsNum, strWhere, orderBy, parameters);
            if (pageList != null && pageList.ContentList.Count() >= 0)
            {
                return pageList;
            }
            return new PagedList<T>();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="rowsNum">每页行数</param>
        /// <param name="strWhere">where条件</param>
        /// <param name="parameters">parameters参数</param>
        /// <returns></returns>
        public PagedList<T> GetCountPage<T>(int rowsNum, string strWhere, object parameters)
        {
            IBaseRepository<T> db = new BaseRepository<T>();
            var pageList = db.GetCountPage( rowsNum, strWhere, parameters);
            if (pageList != null && pageList.RecordCount >= 0)
            {
                return pageList;
            }
            return new PagedList<T>();
        }
        /// <summary>
        /// sql分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="order"></param>
        /// <param name="pageNum"></param>
        /// <param name="rowsNum"></param>
        /// <returns></returns>
        public PagedList<T> GetListPage<T>(string sql, object parameters, string order, int pageNum = 1, int rowsNum = 1)
        {
            IBaseRepository<T> db = new BaseRepository<T>();
            var pageList = db.GetListPage(sql, parameters, order, pageNum, rowsNum); // rowsNum,  order, parameters
            if (pageList != null && pageList.ContentList.Count() >= 0)
            {
                return pageList;
            }
            return new PagedList<T>();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultView Delete<T>(object id, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var result = db.Delete(id, transaction, commandTimeout);
                if (result)
                {
                    return new ResultView { Info = null, Result = 1, ResultMessage = "删除成功" };
                }
                else
                {
                    return new ResultView { Info = null, Result = 0, ResultMessage = "删除失败" };
                }
            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "删除失败e：" + e.Message }; }
        }

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultView DeleteList<T>(string strWhere, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var result = db.DeleteList(strWhere, parameters, transaction,commandTimeout);
                if (result)
                {
                    return new ResultView { Info = null, Result = 1, ResultMessage = "删除成功" };
                }
                else
                {
                    return new ResultView { Info = null, Result = 0, ResultMessage = "删除失败" };
                }
            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "删除失败e：" + e.Message }; }
        }

        /// <summary>
        /// 增加一条数据 （有主键且主键为自增id 返回id）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ResultView Insert<T>(T entity, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var result = db.Insert(entity, transaction, commandTimeout);
                if (result > 0)
                {
                    return new ResultView { Info = result, Result = 1, ResultMessage = "保存成功" };
                }
                else
                {
                    return new ResultView { Info = null, Result = 0, ResultMessage = "保存失败" };
                }
            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "保存失败e：" + e.Message }; }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ResultView Update<T>(T entity,string conditions="", IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var result = db.Update(entity, conditions, transaction, commandTimeout);
                if (result)
                {
                    return new ResultView { Info = null, Result = 1, ResultMessage = "更新成功" };
                }
                else
                {
                    return new ResultView { Info = null, Result = 0, ResultMessage = "更新失败" };
                }
            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "更新失败e：" + e.Message }; }
        }
      /// <summary>
        /// 按条件执行sql 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns>返回影响行数</returns>
        public ResultView Execute<T>(string strSql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                
                IBaseRepository<T> db = new BaseRepository<T>();
                 var result = db.Execute(strSql, parameters, transaction, commandTimeout);
                if (result > 0)
                {
                    return new ResultView { Info = result, Result = 1, ResultMessage = "成功" };
                }
                return new ResultView { Info = null, Result = 0, ResultMessage = "失败" };

            }
            catch (Exception e)
            {
                return new ResultView { Info = null, Result = 0, ResultMessage = "失败e：" + e.Message };
            }
        }



        /// <summary>
        /// 按条件执行sql 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns>返回查询的数据</returns>
        public ResultView ExecuteScalar<T>(string strSql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var result = db.ExecuteScalar(strSql, parameters, transaction, commandTimeout);
                if(result==null)
                    return new ResultView { Info = result, Result = 0, ResultMessage = "失败" };

                return new ResultView { Info = result, Result = 1, ResultMessage = "成功" };

            }
            catch (Exception e)
            {
                return new ResultView { Info = null, Result = 0, ResultMessage = "失败e：" + e.Message };
            }
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strWhere"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ResultView GetList<T>(string strWhere, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var modelList = db.GetModelList(strWhere, parameters, transaction, commandTimeout);
                if (modelList != null && modelList.Count() > 0)
                {
                    return new ResultView { Info = modelList.ToList(), Result = 1, ResultMessage = "请求成功" };
                }

                return new ResultView { Info = null, Result = 0, ResultMessage = "未查询到数据" };

            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "请求车列表失败e：" + e.Message }; }
        }
        /// <summary>
        /// 根据sql请求列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ResultView GetModelList<T>(string sql, object parameters)
        {
            try
            {
                IBaseRepository<T> db = new BaseRepository<T>();
                var modelList = db.GetModelList(sql, parameters);
                if (modelList != null && modelList.Count() > 0)
                {
                    return new ResultView { Info = modelList.ToList(), Result = 1, ResultMessage = "请求成功" };
                }
                return new ResultView { Info = null, Result = 0, ResultMessage = "未请求到数据" };

            }
            catch (Exception e) { return new ResultView { Info = null, Result = 0, ResultMessage = "请求列表失败e：" + e.Message }; }
        }
    }
}
