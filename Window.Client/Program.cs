using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading;
using Window.Client.Client;

namespace Window.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Test();
            Console.Read();
        }

        private void Test()
        {
            int tcpport = int.Parse(ConfigurationSettings.AppSettings["tcpport"]);
            int udpport = int.Parse(ConfigurationSettings.AppSettings["udpport"]);
            string ip = ConfigurationSettings.AppSettings["ip"];
            int receiveBufferSize = int.Parse(ConfigurationSettings.AppSettings["receiveBufferSize"]);
            int sendnumber = 200;
            //string senddata = "我是TCP客户端";
            //byte[] data = Encoding.UTF8.GetBytes(senddata);

       

            ////单个实现测试
            //TcpClient client = new TcpClient(receiveBufferSize, ip, tcpport);
            //for (int i = 0; i < sendnumber; i++)
            //{
                
            //    client.Send(data, 0, data.Length);
            //    Thread.Sleep(300);
            //}
            //senddata = "我是UDP客户端";
            //data = Encoding.UTF8.GetBytes(senddata);
            //UdpClient udpclient = new UdpClient(receiveBufferSize, ip, udpport);
            //IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), udpport);
            //udpclient.Send(iPEndPoint, data);
            //client.Close();

            //多线程测试

            ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            {
                TcpClient client1 = new TcpClient(receiveBufferSize, ip, tcpport);
                for (int i = 0; i < sendnumber; i++)
                {
                    string senddata = "我是TCP客户端:" + tcpport + "指令:" + i;
                    byte[] data = Encoding.UTF8.GetBytes(senddata);
                    client1.Send(data, 0, data.Length);
                    Thread.Sleep(300);
                }
            }));

            ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            {
                TcpClient client1 = new TcpClient(receiveBufferSize, ip, 5556);
                for (int i = 0; i < sendnumber; i++)
                {
                    string senddata = "我是TCP客户端:" + 5556 + "指令:" + i;
                    byte[] data = Encoding.UTF8.GetBytes(senddata);
                    client1.Send(data, 0, data.Length);
                    Thread.Sleep(300);
                }
            }));
            ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            {
                TcpClient client1 = new TcpClient(receiveBufferSize, ip, 5557);
                for (int i = 0; i < sendnumber; i++)
                {
                    string senddata = "我是TCP客户端:" + 5557 + "指令:" + i;
                    byte[] data = Encoding.UTF8.GetBytes(senddata);
                    client1.Send(data, 0, data.Length);
                    Thread.Sleep(300);
                }
            }));
            ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            {
                TcpClient client1 = new TcpClient(receiveBufferSize, ip, 5558);
                for (int i = 0; i < sendnumber; i++)
                {
                    string senddata = "我是TCP客户端:" + 5558 + "指令:" + i;
                    byte[] data = Encoding.UTF8.GetBytes(senddata);
                    client1.Send(data, 0, data.Length);
                    Thread.Sleep(300);
                }
            }));


        }
    }
}
