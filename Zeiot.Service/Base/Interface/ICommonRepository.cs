using Zeiot.Model.Base;
using System.Data;

namespace Zeiot.Service.Base.Interface
{
    /// <summary>
    /// 通用接口
    /// </summary>
    public interface ICommonRepository
    {

        /// <summary>
        /// 根据主键获取实体对象(只能有一个主键)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        ResultView GetModelById<T>(object id, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 获取列表数据 
        /// </summary>
        /// <returns></returns>
        ResultView GetList<T>(IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 根据查询条件列表数据
        /// </summary>
        /// <returns></returns>
        ResultView GetList<T>(string strWhere, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 根据查询条件分页查询
        /// </summary>
        /// <param name="pageNum">页码</param>
        /// <param name="rowsNum">每页行数</param>
        /// <param name="strWhere">where条件</param>
        /// <param name="orderBy">Orde by排序</param>
        /// <param name="parameters">parameters参数</param>
        /// <returns></returns>
        PagedList<T> GetListPage<T>(int pageNum, int rowsNum, string strWhere, string orderBy, object parameters);

        /// <summary>
        /// 根据查询条件分页查询 返回总条数
        /// </summary>
        /// <param name="rowsNum">每页行数</param>
        /// <param name="strWhere">where条件</param>
        /// <param name="parameters">parameters参数</param>
        /// <returns></returns>
        PagedList<T> GetCountPage<T>(int rowsNum, string strWhere,  object parameters);

        /// <summary>
        /// 根据sql获取一条数据(可以任意拼接sql 查询出来的数据需要对应的model对象接收)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ResultView GetModel<T>(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 根据sql语句分页查询 (可以任意拼接sql 查询出来的数据需要对应的model对象接收)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">parameters参数</param>
        /// <param name="order">Orde by排序</param>
        /// <param name="pageNum">页码</param>
        /// <param name="rowsNum">每页行数</param>
        /// <returns></returns>
        PagedList<T> GetListPage<T>(string sql, object parameters, string order, int pageNum = 1, int rowsNum = 1);
        /// <summary>
        /// 根据sql获取list集合 (可以任意拼接sql 查询出来的数据需要对应的model对象接收)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ResultView GetModelList<T>(string sql, object parameters);
        /// <summary>
        /// 执行sql语句 根据受影响的行数判断是否成功
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ResultView Execute<T>(string strSql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = default(int?));

        /// <summary>
        /// 执行sql语句 返回首行首列数据
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ResultView ExecuteScalar<T>(string strSql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = default(int?));
        /// <summary>
        /// 添加一条数据 （有主键且主键为自增id 返回id）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        ResultView Insert<T>(T entity, IDbTransaction transaction = null, int? commandTimeout = default(int?));


        /// <summary>
        /// 根据id删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        ResultView Delete<T>(object id, IDbTransaction transaction = null, int? commandTimeout = default(int?));

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <returns></returns>
        ResultView DeleteList<T>(string strWhere, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?));

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="conditions">where条件（无主键时必填），且优先级高于主键查询</param>
        /// <returns></returns>
        ResultView Update<T>(T entity,string conditions="", IDbTransaction transaction = null, int? commandTimeout = default(int?));

      
    }
}
