using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Model.Base
{
    /// <summary>
    /// 校验码对象
    /// </summary>
    public class ValidateCodeModel
    {
        /// <summary>
        /// 校验码是否可用
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 请求次数 
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
    }
}
