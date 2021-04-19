
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Window.Server.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class TimerManager
    {
        System.Timers.Timer _timer = new System.Timers.Timer();
        TcpManager tcp;

        /// <summary>
        /// 
        /// </summary>
        public TimerManager(TcpManager _tcp)
        {
            tcp = _tcp;
            Thread t = new Thread(Load);
            t.Start();
            TxtLogHelper.WriteLog("启动定时任务");
        }
        /// <summary>
        /// 启动定时
        /// </summary>
        private void Load()
        {
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(RunMethod);
            _timer.Interval = 1 * 60 * 60 * 1000;//单位毫秒  每小时执行一次
            _timer.Start();
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        /// <summary>
        /// 调用方法
        /// </summary>
        private void RunMethod(object sender, System.Timers.ElapsedEventArgs e)
        {
            TxtLogHelper.WriteLog("开始执行定时任务");
            _timer.Enabled = false;
            try
            {
                DeleteLog();
            }
            catch (Exception ex)
            {
                TxtLogHelper.WriteLog("执行定时任务异常："+ex.Message);
            }
            TxtLogHelper.WriteLog("定时任务执行结束");
            _timer.Enabled = true;
        }

        #region 定时任务方法
        #region 定时删除日志
        private void DeleteLog()
        {
            try
            {
                TxtLogHelper.WriteLog("执行删除日志任务");
                DateTime date = DateTime.Now;
                if (date.Hour > 1)//一点前执行
                {
                    return;
                }
                string path = System.Environment.CurrentDirectory + @"\\Log\\TCP\\";
                if (Directory.Exists(path))
                    DeleteFile(path, 3); //删除该目录下 超过 3天的文件
                path = System.Environment.CurrentDirectory + @"\\Log\\UDP\\";
                if (Directory.Exists(path))
                    DeleteFile(path, 7); //删除该目录下 超过 7天的文件
                path = System.Environment.CurrentDirectory + @"\\Log\\Info\\";
                if (Directory.Exists(path))
                    DeleteFile(path, 14); //删除该目录下 超过 14天的文件

            }
            catch (Exception ex)
            {
                TxtLogHelper.WriteLog("执行删除日志任务异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除目录下超过指定天数的文件
        /// </summary>
        private void DeleteFile(string fileDirect, int saveDay)
        {
            DateTime nowTime = DateTime.Now;
            string[] files = Directory.GetFiles(fileDirect, "*.log", SearchOption.AllDirectories);  //获取该目录下所有 .txt文件
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                TimeSpan t = nowTime - fileInfo.CreationTime;  //当前时间  减去 文件创建时间
                int day = t.Days;
                if (day > saveDay)   //保存的时间，单位：天
                {
                    try
                    {
                        System.IO.File.Delete(fileInfo.FullName); //删除文件
                    }
                    catch (Exception ex)
                    {
                        TxtLogHelper.WriteLog("删除日志文件异常：" + ex.Message);
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}
