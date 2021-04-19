using Zeiot.Model.Base;
using System.Collections.Generic;
using System.Data;
using System;

namespace Zeiot.Service.Base.Interface
{
    /// <summary>
    /// 接口层Device
    /// </summary>
    public interface IBaseRepository<T>
    {
        #region  成员方法
        /// <summary>
        /// 增加一条数据 （有主键且主键为自增id 返回id）
        /// </summary>
        int Insert(T model, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 根据ID删除一条数据
        /// </summary>
        bool Delete(object Id, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <returns></returns>
        bool DeleteList(string strWhere, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 更新一条数据
        /// </summary>
        bool Update(T model,string conditions="", IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 根据sql获取实体对象
        /// </summary>
        T GetModel(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 根据ID获取实体对象
        /// </summary>
        T GetModelById(object Id, IDbTransaction transaction = null, int? commandTimeout = default(int?));

        //T GetModel(string strWhere, object parameters);
        /// <summary>
        /// 根据条件获取实体对象集合
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetModelList(string strWhere, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 根据sql 获取集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<T> GetModelList(string sql, object parameters);
        /// <summary>
        /// 根据sql表获取用户的数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="order"></param>
        /// <param name="pageNum"></param>
        /// <param name="rowsNum"></param>
        /// <returns></returns>
        PagedList<T> GetListPage(string sql, object parameters, string order, int pageNum = 1, int rowsNum = 1);          
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageNum">页码</param>
        /// <param name="rowsNum">每页行数</param>
        /// <param name="strWhere">where条件</param>
        /// <param name="orderBy">Orde by排序</param>
        /// <param name="parameters">parameters参数</param>
        /// <returns></returns>
        PagedList<T> GetListPage(int pageNum, int rowsNum, string strWhere, string orderBy, object parameters);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="rowsNum">每页行数</param>
        /// <param name="strWhere">where条件</param>
        /// <param name="parameters">parameters参数</param>
        /// <returns></returns>
        PagedList<T> GetCountPage( int rowsNum, string strWhere, object parameters);

        /// <summary>
        ///  按条件执行sql
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        int Execute(string strSql, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?));

        /// <summary>
        ///  按条件执行sql
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        object ExecuteScalar(string strSql, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        #endregion
    }
}
