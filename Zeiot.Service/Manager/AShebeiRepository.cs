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
    public class AShebeiRepository: Repository
    { 

        /// <summary>
        ///构造函数 
        /// </summary>
        public AShebeiRepository()
        {
        }
        #region 删除
        /// <summary>
        ///根据条件删除 
        /// </summary>
        public bool DeleteList(a_shebei Query)
        {
            #region 关键字过滤 ;
            //string 类型需要过滤 ;
            //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
            #endregion ;
            ResultView rv = respository.DeleteList<a_shebei>(" where id = @id  ", Query);
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
        public List<a_shebei> SearchList(a_shebei Query)
        {
            #region 关键字过滤 ;
            //string 类型需要过滤 ;
            //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
            #endregion ;
             ResultView rv = respository.GetList<a_shebei>(" where id = @id ", Query);
            if (rv.Result == 1) 
            { 
               return rv.Info;
             } 
             return null; 
        }
        /// <summary>
        ///根据id查询对象 
        /// </summary>
        public a_shebei SearchModel(string id)
        {
             ResultView rv = respository.GetModelById<a_shebei>(id);
            if (rv.Result == 1) 
            { 
               return rv.Info;
             } 
             return null; 
        }
        /// <summary>
        ///根据条件查询对象 
        /// </summary>
        public a_shebei SearchModelByXlh(string xlh)
        {
            #region 关键字过滤 ;
            //string 类型需要过滤 ;
            //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
            #endregion ;
             ResultView rv = respository.GetModel<a_shebei>(" select * from a_shebei where xlh="+ xlh, null);
            if (rv.Result == 1) 
            { 
               return rv.Info;
             } 
             return null; 
        }
        /// <summary>
        ///分页查询列表 
        /// </summary>
        /// <param name="Query">查询条件</param>
        /// <param name="pageindex">第几页</param>
        /// <param name="pagecount">每页条数</param>
        public AShebei_View SearchListPage(a_shebei Query, int pageindex = 1, int pagecount = 10)
        {
            AShebei_View model = new AShebei_View(); 
           #region 关键字过滤 ;
           //string 类型需要过滤 ;
           //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
           #endregion ;
           var rv = respository.GetListPage<a_shebei>(pageindex, pagecount, "where id = @id  ", "id", Query);
            model.pagecount = rv.RecordCount; 
            model.AShebei_list=rv.ContentList ;
            if (model.AShebei_list == null)
                model.AShebei_list = new List<a_shebei>();
            return model; 
        }
        /// <summary>
        ///查询数据总条数（分页使用) 
        /// </summary>
        /// <param name="Query">查询条件</param>
        public int SearchListPageCount(a_shebei Query)
        {
            PageInfo model = new PageInfo(); 
           #region 关键字过滤 ;
           //string 类型需要过滤 ;
           //Query.name = "%" + StaticBase.KeyFilter(Query.name) + "%";
           #endregion ;
           var rv = respository.ExecuteScalar<PageInfo>("select count(1) as Count from a_shebei where id = @id  ", Query);
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
