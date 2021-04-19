using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Model.Base
{
    /// <summary>
    /// 页面返回基类
    /// </summary>
    public class BizPage
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msginfo { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int pageindex { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public int pagecount { get; set; }

        /// <summary>
        ///是否超级管理员 1 是 0 否
        /// </summary>
        public int IsSuper { get; set; }
    }
}
