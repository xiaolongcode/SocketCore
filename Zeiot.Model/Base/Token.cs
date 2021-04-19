using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Model.Base
{
    /// <summary>
    /// token对象
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户类型 0 普通用户  1 超级管理员
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 项目id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 用户名对应签名Token
        /// </summary>
        public string SignToken { get; set; }


        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
    }
}
