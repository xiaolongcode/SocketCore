using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Core
{
    public class TxtLogHelper
    {
        #region 日志记录
        /// <summary>
        /// 写普通日志，存放到指定路径，使用默认日志类型
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="_logtype">0 设备回调数据</param>
        public static void WriteLog(string msg, int _logtype)
        {
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Log\\";
            switch (_logtype)
            {
                case 0:
                    fileName += "设备回调数据\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";//
                    break;
                default:
                    fileName += "Info\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                    break;
            }
            string logContext = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + msg;

            WriteFile(logContext, fileName);
        }
        /// <summary>
        /// 写日志到文件
        /// </summary>
        /// <param name="logContext">日志内容</param>
        /// <param name="fullName">文件名</param>
        private static void WriteFile(string logContext, string fullName)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            int splitIndex = fullName.LastIndexOf('\\');
            if (splitIndex == -1) return;
            string path = fullName.Substring(0, splitIndex);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            try
            {
                if (!File.Exists(fullName)) fs = new FileStream(fullName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
                else fs = new FileStream(fullName, FileMode.Append);

                sw = new StreamWriter(fs);
                logContext += "\r\n";
                sw.WriteLine(logContext);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 写普通日志，存放到默认路径，使用默认日志类型
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void WriteLog(string msg)
        {
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Log\\Info\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg;
            WriteFile(msg, "");
        }
        #endregion
    }
}
