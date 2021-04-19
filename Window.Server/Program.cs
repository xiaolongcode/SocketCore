using Window.Server.Server;
using System;
using System.Threading;
using Zeiot.Service.Base.Instrument;

namespace Window.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int tcpport = int.Parse(AppSettingHelper.GetAppSetting("tcpport"));
            int numConnections = int.Parse(AppSettingHelper.GetAppSetting("numConnections"));
            int receiveBufferSize = int.Parse(AppSettingHelper.GetAppSetting("receiveBufferSize"));
            int overtime = int.Parse(AppSettingHelper.GetAppSetting("overtime"));
            int udpport = int.Parse(AppSettingHelper.GetAppSetting("udpport"));
            string istimer = AppSettingHelper.GetAppSetting("isTimer");
            SimpleCRUD.SetConnectionString("", 0);

            Thread tlog = new Thread(TxtLogHelper.LoadData);
            tlog.Start();
            TcpManager tcp = new TcpManager(numConnections, receiveBufferSize, overtime, tcpport);
            UdpManager udp = new UdpManager(receiveBufferSize, udpport, tcp);
            if (istimer == "1")
            {
                TimerManager tim = new TimerManager(tcp);
            }
            //ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            //{
            //    TcpManager push1 = new TcpManager(numConnections, receiveBufferSize, overtime, 5556);
            //}));
            //ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            //{
            //    TcpManager push1 = new TcpManager(numConnections, receiveBufferSize, overtime, 5557);
            //}));
            //ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            //{
            //    TcpManager push1 = new TcpManager(numConnections, receiveBufferSize, overtime, 5558);
            //}));
            Console.Read();
        }
    }
}
