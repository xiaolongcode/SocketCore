using System.ComponentModel;

namespace Zeiot.Model.Base
{
    /// <summary>
    /// API返回统一结果
    /// </summary>
    [Description("API返回统一结果")]
    public class ResultView
    {
        /// <summary>
        /// 生成实列
        /// </summary>
        /// <param name="info">实体model</param>
        /// <param name="result">0：失败，1：成功，2：session过期，其他自定义</param>
        /// <param name="resultMessage">返回提示</param>
        /// <returns></returns>
        public static ResultView GetResult(dynamic info, int result, string resultMessage)
        {
            ResultView view = new ResultView
            {
                Result = result,
                Info = info,
                ResultMessage = resultMessage
            };
            return view;
        }
        /// <summary>
        /// 返回执行状态(0：失败，1：成功，2：session过期，其他自定义)
        /// </summary>
        public int Result { get; set; }
        /// <summary>
        /// 返回的数据对象
        /// </summary>
        [Description("返回的数据对象")]
        public dynamic Info { get; set; }
        /// <summary>
        /// 返回操作提示信息
        /// </summary>
        public string ResultMessage { get; set; }
    }
}
