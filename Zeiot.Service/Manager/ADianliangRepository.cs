//======================================================================
//      Copyright (c) 2021-04-08 Zeiot All rights reserved. 
//======================================================================
using System.Collections.Generic;
using Zeiot.Service.Manager.Base;
using Zeiot.Model.Base;
using Zeiot.Model.ViewContext;
using Zeiot.Model.DBContext;

namespace Zeiot.Service.Manager
 { 
    /// <summary> 
    /// 
    /// <summary> 
    public class ADianliangRepository: Repository
    { 

        /// <summary>
        ///构造函数 
        /// </summary>
        public ADianliangRepository()
        {
        }
        #region 删除
        /// <summary>
        ///根据条件删除 
        /// </summary>
        public bool DeleteList(a_dianliang Query)
        {
            #region 关键字过滤 ;
            //string 类型需要过滤 ;
            //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
            #endregion ;
            ResultView rv = respository.DeleteList<a_dianliang>(" where id = @id  ", Query);
            if (rv.Result == 1) 
            { 
               return true; 
             } 
             return false; 
        }
        #endregion
        #region 查询
        /// <summary>
        ///根据条件查询列表 
        /// </summary>
        public List<a_dianliang> SearchList(a_dianliang Query)
        {
            #region 关键字过滤 ;
            //string 类型需要过滤 ;
            //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
            #endregion ;
            ResultView rv = respository.GetList<a_dianliang>(" where id = @id ", Query);
            if (rv.Result == 1)
            {
                return rv.Info;
            }
            return null;
        }
        /// <summary>
        ///根据id查询对象 
        /// </summary>
        public a_dianliang SearchModel(string id)
        {
            ResultView rv = respository.GetModelById<a_dianliang>(id);
            if (rv.Result == 1)
            {
                return rv.Info;
            }
            return null;
        }
        /// <summary>
        ///根据条件查询对象 
        /// </summary>
        public a_dianliang SearchModel(a_dianliang Query)
        {
            #region 关键字过滤 ;
            //string 类型需要过滤 ;
            //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
            #endregion ;
            ResultView rv = respository.GetModel<a_dianliang>(" select * from a_dianliang where id= @id   ", Query);
            if (rv.Result == 1)
            {
                return rv.Info;
            }
            return null;
        }
        /// <summary>
        ///分页查询列表 
        /// </summary>
        /// <param name="Key">关键字</param>
        /// <param name="pageindex">第几页</param>
        /// <param name="pagecount">每页条数</param>
        public ADianliang_View SearchListPage(string Key, int pageindex = 1, int pagecount = 10)
        {
            ADianliang_View model = new ADianliang_View();
            string strwhere = " 1=1 ";
            var rv = respository.GetListPage<a_dianliang>(pageindex, pagecount, "where " + strwhere, "date desc", null);
            model.pagecount = rv.RecordCount;
            model.ADianliang_list = rv.ContentList;
            if (model.ADianliang_list == null)
                model.ADianliang_list = new List<a_dianliang>();
            return model;
        }
        /// <summary>
        ///查询数据总条数（分页使用) 
        /// </summary>
        /// <param name="Key">关键字</param>
        public int SearchListPageCount(string Key)
        {
            PageInfo model = new PageInfo();
            string strwhere = " 1=1 ";//state 0 未审核 1 已审核 -1 已删除

            var rv = respository.ExecuteScalar<PageInfo>("select count(1) as Count from a_dianliang where " + strwhere, null);
            if (rv.Result == 1)
            {
                model.Count = rv.Info;
            }
            if (model == null)
                return 0;
            return model.Count;
        }
        #endregion 
    }
} 
