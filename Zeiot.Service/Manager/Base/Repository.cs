using System;
using System.Collections.Generic;
using Zeiot.Model.Base;
using Zeiot.Service.Base.Implement;
using Zeiot.Service.Base.Interface;


namespace Zeiot.Service.Manager.Base
{
    /// <summary>
    /// 公共的数据库访问操作
    /// </summary>
    public class Repository
    {
        public readonly ICommonRepository respository = null;
        /// <summary>
        /// 
        /// </summary>
        public Repository()
        {
            respository = new CommonRepository();
        }

        #region 增
        /// <summary>
        ///增加一条数据 （有主键且主键为自增id 返回id）
        /// </summary>
        /// <param name="Query">要添加的数据对象</param>
        /// <returns>0 失败 </returns>
        public int Insert<T>(T Query)
        {
            ResultView rv = respository.Insert(Query);
            if (rv.Result == 1)
            {
               return rv.Info;
            }
            return 0;
        }


        #endregion

        #region 删
        /// <summary>
        ///删除 
        /// </summary>
        /// <param name="id">要删除的数据id</param>
        public bool Delete<T>(dynamic id)
        {
            ResultView rv = respository.Delete<T>(id);
            if (rv.Result == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///根据条件删除 
        /// </summary>
        /// <param name="strwhere">查询条件(不需要 where 关键字)</param>
        public bool DeleteList<T>(string strwhere)
        {
            #region 关键字过滤 ;
            //string 类型需要过滤 ;
            //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
            #endregion ;
            ResultView rv = respository.DeleteList<T>(" where "+ strwhere, null);
            if (rv.Result == 1)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 改
        /// <summary>
        ///修改 
        /// </summary>
        /// <param name="Query">要修改的数据对象</param>
        /// <param name="where">判断条件（无主键时必填），且优先级高于主键查询</param>
        public bool Edit<T>(T Query, string where = "")
        {
            ResultView rv = respository.Update(Query, where);
            if (rv.Result == 1)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 查
        /// <summary>
        ///根据主键获取实体对象(只能有一个主键) 
        /// </summary>
        /// <param name="id">id</param>
        public ResultView GetModelById<T>(string id)
        {
            return respository.GetModelById<T>(id);
        }
        /// <summary>
        ///查询所有数据 
        /// </summary>
        public ResultView List<T>()
        {
            return respository.GetList<T>();
        }
        /// <summary>
        ///查询所有数据 
        /// </summary>
        public List<T> ListAll<T>()
        {
            ResultView rv= respository.GetList<T>();
            if (rv.Result == 1)
            {
                return rv.Info;
            }
            return new List<T>();
        }
        /// <summary>
        ///查询所有数据 
        /// </summary>
        public List<T> GetModelList<T>(string sql)
        {
            ResultView rvm = respository.GetModelList<T>(sql, null);

            if (rvm.Result == 1)
            {
              return rvm.Info;
            }
            return null;
        }
        /// <summary>
        ///查询根据条件数据 
        /// </summary>
        /// <param name="strwhere">查询条件(不需要 where 关键字)</param>
        public List<T> List<T>(string strwhere)
        {
            ResultView rv = respository.GetList<T>(strwhere, null);
            if (rv.Result == 1)
            {
                return rv.Info;
            }
            return new List<T>();
        }
        /// <summary>
        ///分页查询--列表查询  未开放
        /// </summary>
        /// <param name="strwhere">查询条件(不需要 where 关键字)</param>
        /// <param name="pageindex">第几页</param>
        /// <param name="pagecount">每页条数</param>
        /// <param name="orderby">排序</param>
        private PageInfo ListPage<T>(string strwhere, int pageindex = 1, int pagecount = 10, string orderby = "id")
        {
            PageInfo model = new PageInfo();
            if (!string.IsNullOrEmpty(strwhere))
            {
                if (!strwhere.ToLower().Contains("where"))
                {
                    strwhere = " where " + strwhere;
                }
            }
            strwhere = StaticBase.WhereFilter(strwhere);
            var rv = respository.GetListPage<T>(pageindex, pagecount, strwhere, orderby, null);
            model.Count = rv.RecordCount;
            model.Info = rv.ContentList;
            return model;
        }
        /// <summary>
        ///分页查询--数据总量查询  未开放
        /// </summary>
        /// <param name="strwhere">查询条件(不需要 where 关键字)</param>
        /// <param name="pagecount">每页条数</param>
        private PageInfo ListCount<T>(string strwhere, int pagecount = 10)
        {
            PageInfo model = new PageInfo();
            if (!string.IsNullOrEmpty(strwhere))
                strwhere = " where " + strwhere;
            strwhere = StaticBase.WhereFilter(strwhere);
            var rv = respository.GetCountPage<T>(pagecount, strwhere, null);
            model.Count = rv.RecordCount;
            return model;
        }

        /// <summary>
        ///返回首行列数据
        /// </summary>
        ///<param name="strwhere">查询条件(不需要 where 关键字)</param>
        ///<param name="columnName">要查询的列名 默认为查询条数</param>
        /// <returns>返回首行首列数据</returns>
        public dynamic Scalar<T>(string strwhere,string columnName= " count(1) ")
        {
            strwhere = StaticBase.SqlFilter(strwhere, 0);
            Type t = typeof(T);

            string strSql = "select "+ columnName + " from " + t.Name;
            if (strwhere.ToLower().Contains("where"))
            {
                strSql += strwhere;
            }
            else
            {
                strSql += " where " + strwhere;
            }
            var rv = respository.ExecuteScalar<int>(strSql);
            if (rv.Result == 1)
            {
                return rv.Info;
            }
            return null;
        }

        /// <summary>
        ///根据查询条件判断数据是否存在
        /// </summary>
        /// <param name="strwhere">查询条件(不需要 where 关键字)</param>
        /// <returns>true false</returns>
        public bool Exist<T>(string strwhere)
        {
            strwhere = StaticBase.SqlFilter(strwhere, 0);
            Type t = typeof(T);
            string strSql = "select count(1) from " + t.Name ;
            if (strwhere.ToLower().Contains("where"))
            {
                strSql += strwhere;
            }
            else
            {
                strSql += " where " + strwhere;
            }
            var rv = respository.ExecuteScalar<int>(strSql);
            if (rv.Result == 1)
            {
                int count = rv.Info;
                if (count > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///执行一条sql语句
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="isone">true 一条sql语句  </param>
        /// <param name="type">0 查询 1 添加 2 修改 3 删除</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string strSql, bool isone, int type)
        {
            int count = 0;
            strSql = StaticBase.SqlFilter(strSql, type, isone);
            var rv = respository.Execute<int>(strSql);
            if (rv.Result == 1)
            {
                count = rv.Info;
            }
            return count;
        }
        #endregion
    }
}
