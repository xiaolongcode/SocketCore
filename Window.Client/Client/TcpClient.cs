
using socket.Core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Window.Client.Client
{
    public class TcpClient
    {
        private TcpPushClient client;
        Random random = new Random();
        public TcpClient(int receiveBufferSize, string ip, int port)
        {
            client = new TcpPushClient(receiveBufferSize);
            client.OnConnect += Client_OnConnect;
            client.OnReceive += Client_OnReceive;
            client.OnSend += Client_OnSend;
            client.OnClose += Client_OnClose;
            client.Connect(ip, port);
        }

        private void Client_OnClose()
        {
            Console.WriteLine($"Tcp断开");
        }

        private void Client_OnReceive(byte[] data)
        {
            string receiveStr = System.Text.Encoding.UTF8.GetString(data);
            Console.WriteLine($"Tcp接收长度[{data.Length}]     {random.Next(1, 9999)}");
            Console.WriteLine($"Tcp服务端回复内容[{receiveStr}]     {random.Next(1, 9999)}");
        }

        private void Client_OnConnect(bool success)
        {
            Console.WriteLine($"Tcp连接{success}");
        }

        private void Client_OnSend(int Length)
        {
            Console.WriteLine($"Tcp已发送长度{Length}    {random.Next(1, 9999)}");
        }

        public void Send(byte[] data, int offset, int length)
        {
            string receiveStr = System.Text.Encoding.UTF8.GetString(data);
            Console.WriteLine($"Tcp发送内容{receiveStr}");
            client.Send(data, offset, length);
        }

        public void Close()
        {
            client.Close();
        }

    }
}
