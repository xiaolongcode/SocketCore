using socket.Core.Client;
using socket.Core.Server;
using System;
using System.Net;

namespace Window.Client.Client
{
    public class UdpClient
    {
        UdpClients client;
        Random random = new Random();
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="ip">ip或者域名</param>
        /// <param name="port">端口</param>
        public UdpClient(int receiveBufferSize, string ip, int port)
        {
            client = new UdpClients(receiveBufferSize);
            client.OnReceive += OnReceive;
            client.OnSend += OnSend;
            client.Start(ip,port);
            Console.WriteLine("Udp已启动，端口：" + port);
        }
        /// <summary>
        /// Udp发送后事件
        /// </summary>
        /// <param name="length">已发送长度</param>
        private void OnSend( int length)
        {
            Console.WriteLine($"Udp已发送长度{length}    {random.Next(1, 9999)}");
        }
        /// <summary>
        /// Udp接收数据事件
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        private void OnReceive( byte[] data, int offset, int length)
        {
            string receiveStr = System.Text.Encoding.UTF8.GetString(data);
            Console.WriteLine($"Udp接收长度[{data.Length}]     {random.Next(1, 9999)}");
            Console.WriteLine($"Udp服务端回复内容[{receiveStr}]     {random.Next(1, 9999)}");
        }
        /// <summary>
        /// Udp发送
        /// </summary>
        /// <param name="remoteEndPoint">远程ip与端口</param>
        /// <param name="data">数据</param>
        public void Send(EndPoint remoteEndPoint, byte[] data)
        {
            Console.WriteLine($"Udp发送长度[{data.Length}]     {random.Next(1, 9999)}");
            string receiveStr = System.Text.Encoding.UTF8.GetString(data);
            Console.WriteLine($"Udp发送内容{receiveStr}");
            client.Send(remoteEndPoint, data, 0, data.Length);
        }
    }
}
