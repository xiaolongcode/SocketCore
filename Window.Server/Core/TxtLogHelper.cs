using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Window.Server
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class TxtLogHelper
    {
        private static Queue<LogModel> loglist = new Queue<LogModel>();
        private static System.Timers.Timer timer = new System.Timers.Timer();
        /// <summary>
        /// 初始化
        /// </summary>
        public static void LoadData()
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(WriteTimer);
            timer.Interval = 10 * 1000;
            timer.Start();
            timer.AutoReset = true;
            timer.Enabled = true;
            TxtLogHelper.WriteLog("启动日志任务");
        }
        private static void WriteTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Enabled = false;
            try
            {
                while (loglist.Count > 0)
                {
                    LogModel model = loglist.Dequeue();
                    try
                    {
                        WriteFile(model.msg, model.path);
                    }
                    catch
                    {
                        if (model.count <= 3)
                        {
                            model.count += 1;
                            loglist.Enqueue(model);
                        }

                    }
                }
            }
            catch
            {

            }
            timer.Enabled = true;
        }
        /// <summary>
        /// 写普通日志，存放到指定路径
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="path">目录 默认为Info目录</param>
        /// <param name="savetype">保存文件类型 0 按天创建文件  1 按小时创建文件</param>
        public static void WriteLog(string msg, string path = "Info", int savetype = 0)
        {
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Log\\" + path + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            if (savetype == 1)
            {
                fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Log\\" + path + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".log";
            }
            string logContext = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + msg;

            LogModel model = new LogModel();
            model.path = fileName;
            model.msg = logContext;
            model.count = 1;
            loglist.Enqueue(model);
            //WriteFile(logContext, fileName);
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

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

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



    }

    public class LogModel
    {
        public string msg { get; set; }
        public string path { get; set; }
        
        public int count { get; set; }
        
    }
}
