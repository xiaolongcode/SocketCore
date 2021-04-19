namespace Zeiot.Service
{
    /// <summary>
    /// 初始化加载数据
    /// </summary>
    public class StaticBase
    {
        /// <summary>
        ///  参数关键字过滤
        /// </summary>
        /// <param name="key">参数值</param>
        /// <returns></returns>
        public static string KeyFilter(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string newkey = key.ToLower();
                //注释符号替换为横杠
                newkey = newkey.Replace("--", "——");
                //单引号替换成两个单引号
                newkey = newkey.Replace("'", "''");
                //半角括号替换为全角括号
                newkey = newkey.Replace("(", "（");
                newkey = newkey.Replace(")", "）");
                //去除执行存储过程的命令关键字
                newkey = newkey.Replace("exec", "");
                newkey = newkey.Replace("execute", "");
                //去除系统存储过程或扩展存储过程关键字
                newkey = newkey.Replace("xp_", "x p_");
                newkey = newkey.Replace("sp_", "s p_");
                //防止16进制注入
                newkey = newkey.Replace("0x", "0 x");
                //半角封号替换为全角封号，防止多语句执行
                newkey = newkey.Replace(";", "；");
                //去除关键字
                newkey = newkey.Replace("delete", "");
                newkey = newkey.Replace("update", "");
                newkey = newkey.Replace("insert", "");
                return newkey;
            }
            return key;
        }
        /// <summary>
        /// Where条件关键字过滤
        /// </summary>
        /// <param name="strwhere">查询条件</param>
        /// <returns></returns>
        public static string WhereFilter(string strwhere)
        {

            if (!string.IsNullOrEmpty(strwhere))
            {
                string newstrwhere = strwhere;
                //注释符号替换为横杠
                newstrwhere = newstrwhere.Replace("--", "——");
                //半角括号替换为全角括号
                newstrwhere = newstrwhere.Replace("(", "（");
                newstrwhere = newstrwhere.Replace(")", "）");
                //去除执行存储过程的命令关键字
                newstrwhere = newstrwhere.Replace("exec", "");
                newstrwhere = newstrwhere.Replace("execute", "");
                //去除系统存储过程或扩展存储过程关键字
                newstrwhere = newstrwhere.Replace("xp_", "x p_");
                newstrwhere = newstrwhere.Replace("sp_", "s p_");
                //防止16进制注入
                newstrwhere = newstrwhere.Replace("0x", "0 x");
                //半角封号替换为全角封号，防止多语句执行
                newstrwhere = newstrwhere.Replace(";", "；");
                //去除关键字
                newstrwhere = newstrwhere.Replace("delete", "");
                newstrwhere = newstrwhere.Replace("update", "");
                newstrwhere = newstrwhere.Replace("insert", "");
                return newstrwhere;
            }
            return strwhere;
        }
        /// <summary>
        /// sql语句关键字过滤
        /// </summary>
        /// <param name="strsql">sql语句</param>
        /// <param name="type">0 查询 1 添加 2 修改 3 删除</param>
        /// <param name="isone">默认为一条sql语句</param>
        /// <returns></returns>
        public static string SqlFilter(string strsql, int type, bool isone = true)
        {

            if (!string.IsNullOrEmpty(strsql))
            {
                string newstrsql = strsql;
                //注释符号替换为横杠
                newstrsql = newstrsql.Replace("--", "——");
                //半角括号替换为全角括号
                //newstrsql = newstrsql.Replace("(", "（");
                //newstrsql = newstrsql.Replace(")", "）");
                //去除执行存储过程的命令关键字
                newstrsql = newstrsql.Replace("exec", "");
                newstrsql = newstrsql.Replace("execute", "");
                //去除系统存储过程或扩展存储过程关键字
                newstrsql = newstrsql.Replace("xp_", "x p_");
                newstrsql = newstrsql.Replace("sp_", "s p_");
                //防止16进制注入
                newstrsql = newstrsql.Replace("0x", "0 x");

                if (isone)
                {
                    //半角封号替换为全角封号，防止多语句执行
                    newstrsql = newstrsql.Replace(";", "；");
                }
                //去除关键字
                switch (type)
                {
                    case 1:
                        newstrsql = newstrsql.Replace("delete", "");
                        newstrsql = newstrsql.Replace("update", "");
                        break;
                    case 2:
                        newstrsql = newstrsql.Replace("delete", "");
                        newstrsql = newstrsql.Replace("insert", "");
                        break;
                    case 3:
                        newstrsql = newstrsql.Replace("update", "");
                        newstrsql = newstrsql.Replace("insert", "");
                        break;
                    default:
                        newstrsql = newstrsql.Replace("delete", "");
                        newstrsql = newstrsql.Replace("update", "");
                        newstrsql = newstrsql.Replace("insert", "");
                        break;
                }
                return newstrsql;
            }
            return strsql;
        }
    }
}
