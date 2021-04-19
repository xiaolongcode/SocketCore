using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zeiot.Core
{
    public class CacheHelper
    {
        //Add 存在相同的键会异常，返回缓存成功的对象
        //Insert 存在相同的键会替换原值，无返回值

        /// <summary>
        /// Insert 存在相同的键会替换原值 晚上12点的时候缓存被清除
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存的数据</param>
        /// <param name="num">缓存分钟数据 默认5分钟</pakeyram>
        /// <returns></returns>
        public static void Insert(string key,object value,int num=5)
        {
            HttpRuntime.Cache.Insert(key,value, null, DateTime.Now.AddMinutes(num), TimeSpan.Zero);
        }

        /// <summary>
        /// 根据key获取缓存的数据
        /// </summary>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
          return (T)HttpRuntime.Cache.Get(key);
        }

        /// <summary>
        /// 根据key获取缓存的数据
        /// </summary>
        /// <returns></returns>
        public static void Remove(string key)
        {
           HttpRuntime.Cache.Remove(key);
        }
    }
}
