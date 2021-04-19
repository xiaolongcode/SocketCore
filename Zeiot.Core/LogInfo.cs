using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWork.Utility
{
    /// <summary>
    /// 日志打印类
    /// </summary>
    public class LogInfo
    {
        string FilePath =  System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Log\\";
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Platform">平台编号</param>
        public LogInfo(string Platform)
        {
            if (string.IsNullOrEmpty(Platform))
                Platform = "0000";
            FilePath = FilePath + Platform + "\\";
            if (!Directory.Exists(FilePath + "Info"))
                Directory.CreateDirectory(FilePath + "Info");
            if (!Directory.Exists(FilePath + "Error"))
                Directory.CreateDirectory(FilePath + "Error");
        }
        /// <summary>
        /// 普通日志 记录操作记录
        /// </summary>
        /// <returns></returns>
        public bool Info(string message)
        {
            try
            {
                DateTime newdate = DateTime.Now;
                string filename = "\\Info\\" + newdate.ToString("yyyyMMddHH") + ".txt"; ;
                using (FileStream stream = new FileStream(FilePath + filename, FileMode.Append))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(newdate.ToString("yyyy-MM-dd HH:mm:ss ：") + message);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(FilePath + "Error"))
                    Directory.CreateDirectory(FilePath + "Error");
                Error("记录日志出现异常", ex);
                return false;
            }

        }

        /// <summary>
        /// 错误日志 记录异常和错误信息
        /// </summary>
        /// <returns></returns>
        public bool Error(string message, Exception exception)
        {
            DateTime newdate = DateTime.Now;
            string filename = "\\Error\\" + newdate.ToString("yyyyMMddHH") + ".txt"; ;
            using (FileStream stream = new FileStream(FilePath + filename, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(newdate.ToString("yyyy-MM-dd HH:mm:ss  message：") + message);
                writer.WriteLine(" Exception：" + exception.Message);
            }
            return false;
        }


    }
}
