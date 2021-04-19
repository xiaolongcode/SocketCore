using socket.Core.Server;
using System;
using System.Net;
using System.Text;
using Zeiot.Core;

namespace Window.Server
{

    public class UdpManager
    {
        UdpServer server;
        TcpManager tcp;
        #region UDP事件
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="port">端口</param>
        public UdpManager(int receiveBufferSize, int port, TcpManager _tcp)
        {
            tcp = _tcp;
            server = new UdpServer(receiveBufferSize);
            server.OnReceive += OnReceive;
            server.OnSend += OnSend;
            server.Start(port);
            Console.WriteLine("Udp监听已启动，端口：" + port);
            TxtLogHelper.WriteLog("Udp监听已启动，端口：" + port);
        }
        /// <summary>
        /// Udp发送后事件
        /// </summary>
        /// <param name="remoteEndPoint">远程ip与端口</param>
        /// <param name="length">已发送长度</param>
        private void OnSend(EndPoint remoteEndPoint, int length)
        {
            Console.WriteLine($"Udp已发送:{remoteEndPoint.ToString()} 长度:{length}");
            TxtLogHelper.WriteLog($"Udp{remoteEndPoint.ToString()}指令已发送", "UDP");
        }
        /// <summary>
        /// Udp接收数据事件
        /// </summary>
        /// <param name="remoteEndPoint">远程ip与端口</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        private void OnReceive(EndPoint remoteEndPoint, byte[] Receive, int offset, int length)
        {
            //去掉自动补0的缓冲区
            byte[] data=new byte[length]; 
            for (int i = 0; i < length; i++)
            {
                data[i] = Receive[i];
            }
            Console.WriteLine($"UDP客户端:{remoteEndPoint.ToString()} 请求长度:{data.Length}");
            TxtLogHelper.WriteLog($"客户端{remoteEndPoint.ToString()}请求长度：{data.Length}", "UDP");

            string key;
            byte[] TcpSendData;
            byte[] UdpSendData = MessageHandling(data, out TcpSendData, out key);

            if (!string.IsNullOrEmpty(key) && TcpSendData != null && TcpSendData.Length > 0)
            {
                if (tcp.TcpSend(key, TcpSendData))
                {
                    TxtLogHelper.WriteLog($"发送TCP指令成功", "UDP");
                }
                else
                {
                    TxtLogHelper.WriteLog($"发送TCP指令失败", "UDP");
                }
            }
            if (UdpSendData != null && UdpSendData.Length >0)
            {
                server.Send(remoteEndPoint, UdpSendData, offset, UdpSendData.Length);
                TxtLogHelper.WriteLog($"回复UDP指令成功", "UDP");
            }
        }
        #endregion
        #region UDP接收到数据后处理
        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="data">接收到的数据</param>
        /// <param name="TcpSendData">需要发送的TCP数据 未null则不发送</param>
        /// <param name="key">自定义key 发送TCP指令判断设备使用</param>
        /// <returns>需要回复的UDP数据 未null则不发送</returns>
        private byte[] MessageHandling(byte[] data,out byte[] TcpSendData, out string key)
        {
            key = "";
            TcpSendData = null;
            byte[] UdpSenddata = null;
            try
            {
                #region 解析数据
                if (data != null && data.Length >0)
                {
                    string getD = ByteHelper.ByteToString(data);
                    Console.WriteLine("UDP客户端请求内容:" + getD);
                    TxtLogHelper.WriteLog("客户端请求内容:" + getD, "UDP");
                   
                    switch (data[2])
                    {
                        case 0X90:
                            #region Modbus数据下发
                            key = data[3].ToString("X");
                            TxtLogHelper.WriteLog("获取到的key：" + key, "UDP");
                            #endregion
                            break;
                    }

                    TxtLogHelper.WriteLog("服务器下发控制命令：" + getD, "UDP");
                    tcp.ConsoleWriteLine("服务器下发控制命令："+getD, 3);
                    TcpSendData = ByteHelper.StringToByte(getD);
                }
                #endregion
            }
            catch (Exception ex)
            {
                key = "";
                TcpSendData = null;
                UdpSenddata = null;
                TxtLogHelper.WriteLog("解析客户端上报命令异常" + ex.Message, "UDP");
            }
            return UdpSenddata;
        }
        #endregion

    }
}
