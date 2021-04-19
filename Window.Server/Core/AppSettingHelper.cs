using System.Configuration;
using System.Linq;

namespace Window.Server
{
    /// <summary>
    /// AppSetting帮助类
    /// </summary>
    public class AppSettingHelper
    {
        /// <summary>
        /// 根据key返回文件中appSettings配置节的value项
        /// </summary>
        /// <param name="strKey">key</param>
        /// <param name="isRefresh">是否重新加载配置数据</param>
        /// <returns></returns>
        public static string GetAppSetting(string strKey, bool isRefresh=false)
        {
            if (isRefresh)
            {
                ConfigurationManager.RefreshSection("appSettings");
            }
            return ConfigurationManager.AppSettings[strKey];
        }
        /// <summary>
        /// 添加keyvalue
        /// </summary>
        /// <param name="strKey">key</param>
        /// <param name="strValue">value</param>
        /// <returns></returns>
        public static bool AddSetting(string strKey, string strValue)
        {
            try
            {
                string[] keys = ConfigurationManager.AppSettings.AllKeys;
                if (keys.Contains(strKey))
                {
                    return false;
                }
                else
                {
                    ConfigurationManager.AppSettings.Add(strKey, strValue);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据key修改value
        /// </summary>
        /// <param name="strKey">key</param>
        /// <param name="strValue">value</param>
        /// <returns></returns>
        public static bool EditSetting(string strKey, string strValue)
        {
            try
            {
                string[] keys = ConfigurationManager.AppSettings.AllKeys;
                if (!keys.Contains(strKey))
                {
                    return false;
                }
                else
                {
                    ConfigurationManager.AppSettings.Set(strKey, strValue);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
