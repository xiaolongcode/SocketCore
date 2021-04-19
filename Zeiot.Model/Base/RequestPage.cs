using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Model.Base
{
    /// <summary>
    /// 页面查询请求对象
    /// </summary>
    public class RequestPage
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 是否删除  0未删除 1已删除  -1 不判断
        /// </summary>
        public int IsDelete { get; set; }
        /// <summary>
        /// 是否起用 0 不启用  1 起用   -1 不判断
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 项目id
        /// </summary>
        public string ProjectId { get; set; }
    }
}
