using System;
using System.Collections.Generic;
using System.Data;

namespace Zeiot.Model.Base
{
    /// <summary>
    /// 分页器类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public PagedList()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public PagedList(IEnumerable<T> contents, int recordCount, int pageCount)
        {
            if (contents != null)
                ContentList = contents is List<T> ? (List<T>)contents : new List<T>(contents);
            RecordCount = recordCount;
            PageCount = pageCount;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static PagedList<T> Instance(IEnumerable<T> contents, int recordCount, int pageCount)
        {

            return new PagedList<T>(contents, recordCount, pageCount);

        }
        ///
        /// 
        #region IPagedList<T> Members
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> ContentList { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public int RecordCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        #endregion
    }

    /// <summary>
    /// 分页器类
    /// </summary>
    public class PagedTable
    {
        /// <summary>
        /// 
        /// </summary>
        public PagedTable()
        {
            Columns = new List<string>();
        }
        /// <summary>
        /// 
        /// </summary>
        public PagedTable(DataTable dt, int recordCount, int pageCount)
        {
            Table = dt;
            RecordCount = recordCount;
            PageCount = pageCount;
            Columns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                Columns.Add(dc.ColumnName);
            }
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static PagedTable Instance(DataTable dt, int recordCount, int pageCount)
        {

            return new PagedTable(dt, recordCount, pageCount);

        }
        ///
        /// 
        #region IPagedTable Members
        public List<string> Columns { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DataTable Table { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RecordCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageCount { get; set; }

        #endregion
    }

    /// <summary>
    /// 分页请求类
    /// </summary>
    public class Paging
    {
        /// <summary>
        /// 
        /// </summary>
        public int PageNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; }
    }
    /// <summary>
    /// 分页查询数据用
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 数据对象
        /// </summary>
        public dynamic Info { get; set; }
    }
}
