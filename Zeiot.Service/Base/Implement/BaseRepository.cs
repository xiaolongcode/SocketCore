using Zeiot.Model.Base;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Zeiot.Service.Base.Interface;
using Zeiot.Service.Base.Instrument;

namespace Zeiot.Service.Base.Implement
{
    /// <summary>
    /// 仓储层基类，通过泛型实现通用的CRUD操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : IBaseRepository<T>
    {
        #region sqlConnct
        /// <summary>
        /// 数据库地址
        /// </summary>
        public readonly static string sqlconnct = SimpleCRUD.sqlconnct;// AppSetting.GetConfig("ConnectionStrings:DefaultConnection");

        /// <summary>
        /// 数据库类型  0 sql server  1 mysql
        /// </summary>
        public readonly static int sqltype = SimpleCRUD.sqltype;

        ///初始化连接对象
        public static SqlConnection conn = null;

        public static MySqlConnection msqconn = null;

        /// <summary>
        /// 初始化连接对象
        /// </summary>
        public IDbConnection OpenConnection()
        {
            if (sqltype == 0)
            {

                conn = new SqlConnection(sqlconnct);
                if (conn.State == ConnectionState.Closed)
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                return conn;
            }
            else
            {

                msqconn = new MySqlConnection(sqlconnct);
                if (msqconn.State == ConnectionState.Closed)
                {
                    try
                    {
                        msqconn.Open();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                return msqconn;
            }
        }
        #endregion

        private IDbConnection _connection;
        #region  成员方法
        /// <summary>
        /// 增加一条数据 （有主键且主键为自增id 返回id）
        /// </summary>
        public int Insert(T model, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            int? result;
            using (_connection = OpenConnection())
            {
                result = _connection.Insert<T>(model, transaction, commandTimeout);

            }
            if (result != null && result > 0)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }
        /// <summary>
        /// 根据ID删除一条数据
        /// </summary>
        public bool Delete(object id, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            int? result;
            using (_connection = OpenConnection())
            {
                result = _connection.Delete<T>(id, transaction, commandTimeout);
            }
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 按条件删除数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool DeleteList(string strWhere, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            int? result;
            using (_connection = OpenConnection())
            {
                result = _connection.DeleteList<T>(strWhere, parameters, transaction, commandTimeout);
            }
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(T model,string conditions="", IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            int? result;
            using (_connection = OpenConnection())
            {
                result = _connection.Update<T>(model,conditions, transaction, commandTimeout);
            }
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据sql获取实体对象
        /// </summary>
        public T GetModel(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            using (_connection = OpenConnection())
            {
                return _connection.QuerySingleOrDefault<T>(sql, parameters,transaction,commandTimeout);
            }
        }
        /// <summary>
        /// 根据主键获取实体对象(只能有一个主键)
        /// </summary>
        public T GetModelById(object id, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            using (_connection = OpenConnection())
            {
                return _connection.Get<T>(id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 根据条件获取实体对象集合
        /// </summary>
        public IEnumerable<T> GetModelList(string strWhere, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            using (_connection = OpenConnection())
            {
                return _connection.GetList<T>(strWhere, parameters, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 根据条件分页获取实体对象集合
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="rowsNum"></param>
        /// <param name="strWhere"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>                   //
        public PagedList<T> GetListPage(int pageNum, int rowsNum, string strWhere, string orderBy, object parameters)
        {
            using (_connection = OpenConnection())
            {
                var entityList = _connection.GetListPaged<T>(pageNum, rowsNum, strWhere, orderBy, parameters);
                var recordCount = _connection.RecordCount<T>(strWhere, parameters);
                var pageCount = (int)Math.Ceiling((recordCount / (double)rowsNum));
                return PagedList<T>.Instance(entityList, recordCount, pageCount);
            }
        }
        /// <summary>
        /// 根据条件分页获取实体对象集合
        /// </summary>
        /// <param name="rowsNum"></param>
        /// <param name="strWhere"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>                   //
        public PagedList<T> GetCountPage(int rowsNum, string strWhere, object parameters)
        {
            using (_connection = OpenConnection())
            {
                var recordCount = _connection.RecordCount<T>(strWhere, parameters);
                var pageCount = (int)Math.Ceiling((recordCount / (double)rowsNum));
                return PagedList<T>.Instance(null, recordCount, pageCount);
            }
        }
        /// <summary>
        /// 根据sql分页获取实体对象集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="order">必填 例如:id</param>
        /// <param name="pageNum"></param>
        /// <param name="rowsNum"></param>
        /// <returns></returns>
        public PagedList<T> GetListPage(string sql, object parameters, string order, int pageNum = 1, int rowsNum = 1)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentException("sql not is Empty");
            if (string.IsNullOrEmpty(order)) throw new ArgumentException("order not is Empty");

            using (_connection = OpenConnection())
            {

                int startNum = rowsNum  * (pageNum-1) + 1;
                int endNum = pageNum * rowsNum;
                string sqlBase = @"SELECT  * FROM (SELECT ROW_NUMBER() OVER(ORDER BY " + order + ") AS PagedNumber, * FROM (" + sql + @") as v ) AS u WHERE  PagedNumber BETWEEN " + startNum + "  AND " + endNum;
                var entityList = _connection.Query<T>(sqlBase, parameters);
                var recordCount = _connection.RecordCount(sql, parameters);
                var pageCount = (int)Math.Ceiling((recordCount / (double)rowsNum));
                return PagedList<T>.Instance(entityList, recordCount, pageCount);
            }
        }
        /// <summary>
        ///  按条件执行sql
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public int Execute(string strSql, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            using (_connection = OpenConnection())
            {
                return _connection.Execute(strSql, parameters, transaction, commandTimeout);

            }
        }

        /// <summary>
        ///  按条件执行sql
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public object ExecuteScalar(string strSql, object parameters, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            using (_connection = OpenConnection())
            {
                return _connection.ExecuteScalar(strSql, parameters, transaction, commandTimeout);

            }
        }
        /// <summary>
        /// 根据sql获取对象集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> GetModelList(string sql, object parameters)
        {
            using (_connection = OpenConnection())
            {

                return _connection.Query<T>(sql, parameters);
            }
        }

        public int Add2(T model, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
