using System.ComponentModel;

namespace Zeiot.Model.Base
{
    /// <summary>
    /// API统一请求对象
    /// </summary>
    [Description("API统一请求对象")]
    public class RequestView
    {
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        ///加密签名串
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 每页条数 分页获取数据用
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 页码 分页获取数据用
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 请求的数据对象
        /// </summary>
        [Description("请求的数据对象")]
        public dynamic Info { get; set; }
    }
}
