
using socket.Core.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zeiot.Core;
using Zeiot.Model.DBContext;
using Zeiot.Service.Manager;

namespace Window.Server
{
    /// <summary>
    /// push 推出数据
    /// 将触发监视事件对象OnReceive（connectId，数据）；数据立即“推送”到应用程序
    /// </summary>
    public class TcpManager
    {
        TcpPushServer server;
        /// <summary>
        /// 自定义客户端列表 key 自定义 value 客户端连接id
        /// </summary>
        internal ConcurrentDictionary<string, int> clientList;
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        /// <param name="port">端口</param>
        public TcpManager(int numConnections, int receiveBufferSize, int overtime, int port)
        {
            server = new TcpPushServer(numConnections, receiveBufferSize, overtime);
            server.OnAccept += Server_OnAccept;
            server.OnReceive += Server_OnReceive;
            server.OnSend += Server_OnSend;
            server.OnClose += Server_OnClose;
            server.Start(port);
            Console.WriteLine("Tcp监听已启动，端口：" + port);
            TxtLogHelper.WriteLog("Tcp监听已启动，端口：" + port);
        }
        #region Tcp事件
        /// <summary>
        /// Tcp连接事件
        /// </summary>
        /// <param name="connectId">客户端连接id</param>
        private void Server_OnAccept(int connectId)
        {
            //server.SetAttached(connectId, 555);
            Console.WriteLine($"Tcp已连接{connectId}");
            TxtLogHelper.WriteLog($"Tcp客户端{connectId}已连接");
        }
        /// <summary>
        /// Tcp发送后事件
        /// </summary>
        /// <param name="connectId">客户端连接id</param>
        /// <param name="length">已发送数据长度</param>
        private void Server_OnSend(int connectId, int length)
        {
            Console.WriteLine($"Tcp已发送:{connectId} 长度:{length}");
            TxtLogHelper.WriteLog($"Tcp客户端{connectId}指令已发送");
        }
        /// <summary>
        /// Tcp连接断开事件
        /// </summary>
        /// <param name="connectId"></param>
        private void Server_OnClose(int connectId)
        {
            int aaa = server.GetAttached<int>(connectId);
            Console.WriteLine($"Tcp断开{connectId}");
            TxtLogHelper.WriteLog($"Tcp客户端{connectId}已断开");
        }
        /// <summary>
        /// Tcp接收数据事件
        /// </summary>
        /// <param name="connectId">客户端连接id</param>
        /// <param name="data">数据</param>
        private void Server_OnReceive(int connectId, byte[] data)
        {
            string Address;
            server.ClientList.TryGetValue(connectId, out Address);
            Console.WriteLine($"客户端连接id:{connectId} 客户端端地址：{Address} 数据长度:{data.Length}");
            TxtLogHelper.WriteLog($"客户端连接id：{connectId} 客户端端地址：{Address} 数据长度：{data.Length}", "TCP");

            string key;
            byte[] senddata = MessageHandling(data, out key);
            if (key.Length > 0)
            {
                AddConnectId(key, connectId);
                int kconnectId = GetConnectId(key);
                TxtLogHelper.WriteLog($"自定义key：{key} key对应客户端id：{kconnectId}", "TCP");
                Console.WriteLine($"自定义key：{key} key对应客户端id：{kconnectId}");
            }
            if (senddata != null && senddata.Length > 0)
            {
                server.Send(connectId, senddata, 0, senddata.Length);
                TxtLogHelper.WriteLog($"成功发送到客户端:{connectId} 数据长度:{senddata.Length}", "TCP");
                Console.WriteLine($"成功发送到客户端:{connectId} 数据长度:{senddata.Length}");
            }
        }
        #endregion

        #region 数据处理方法

        #region TCP接收到数据后处理
        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="data">接收到的数据</param>
        /// <param name="key">自定义key UDP发送TCP指令判断设备使用</param>
        /// <returns>需要发送的数据</returns>
        private byte[] MessageHandling(byte[] data, out string key)
        {
            key = "";
            byte[] Senddata = null;
            try
            {
                #region 解析命令
                if (data.Length > 0)
                {
                    string getD = ByteHelper.ByteToString(data);
                    switch (data[2])
                    {
                        case 0x93:
                            //对时命令
                            TxtLogHelper.WriteLog($"请求内容：{getD}", "TCP");
                            ConsoleWriteLine("接收到的请求内容：" + getD);
                            Senddata = TimeCheck(data);
                            break;
                        case 0x91:
                            //设备数据域上传
                            TxtLogHelper.WriteLog($"请求内容：{getD}", "TCP",1);
                            ConsoleWriteLine("接收到的请求内容：" + getD);
                            Senddata = ModbusUpload(data);
                            key = data[10].ToString("X");
                            TxtLogHelper.WriteLog($"获取到的key：{key}", "TCP", 1);
                            break;
                        case 0x84:
                            //设备注册命令
                            TxtLogHelper.WriteLog($"请求内容：{getD}", "TCP");
                            ConsoleWriteLine("接收到的请求内容：" + getD);
                            Senddata = Register(data);
                            break;
                        case 0x89:
                            TxtLogHelper.WriteLog($"请求内容：{getD}", "TCP",1);
                            ConsoleWriteLine("接收到的请求内容：" + getD);
                            //上传报警设定值
                            Senddata = Register(data);
                            break;
                        default:
                            ConsoleWriteLine("接收到的请求内容：" + getD,2);
                            TxtLogHelper.WriteLog($"请求内容：{getD}", "TCP");
                            //设备回复服务器消息
                            ReplyMessage(data);
                            break;
                    }
                 
                }
                #endregion
            }
            catch (Exception ex)
            {
                key = "";
                Senddata = null;
                TxtLogHelper.WriteLog("解析客户端上报命令异常" + ex.Message, "TCP");
            }
            return Senddata;
        }
        /// <summary>
        /// 对时命令
        /// </summary>
        private byte[] TimeCheck(byte[] data)
        {
            DateTime daydate = DateTime.Now;
            string bates01 = "93," + int.Parse(daydate.ToString("yy")).ToString("X") + "," + daydate.Month.ToString("X") + "," + daydate.Day.ToString("X") + "," + WeekHelper.ConvertDateToWeek(daydate).ToString("00") + "," + daydate.Hour.ToString("X") + "," + daydate.Minute.ToString("X") + "," + daydate.Second.ToString("X");
            string batesstr = "7b,7b," + ByteHelper.ByteToString(ByteHelper.TenToHex(bates01)).Trim(' ').Replace(' ', ',') + "," + ByteHelper.CRC16_String(ByteHelper.TenToHex(bates01)) + ",7d,7d";
            byte[] SendData = ByteHelper.TenToHex(batesstr);//固定值
            TxtLogHelper.WriteLog("对时回复命令内容：" + batesstr, "TCP");
            ConsoleWriteLine("对时回复命令内容：" + batesstr,1);
            return SendData;
        }
        /// <summary>
        /// 设备数据域上传
        /// </summary>
        private byte[] ModbusUpload(byte[] data)
        {
            if (data[5] == data[7] && data[5] == 0x31 && data[9] == data[8] && data[8] == 0x28 && data[11] == 0x03)
            {
                ADianliangRepository dlrep = new ADianliangRepository();
                a_dianliang model = new a_dianliang();
                int index = 10;
                string dz = data[index].ToString();
                index++;
                index++;
                string zjs = data[index].ToString();
                index++;
                string dybh = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));
                index += 2;
                string dlbh = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));
                index += 2;
                string nxwd = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));
                index += 2;
                string sjrq = ("20" + data[index + 5].ToString("X")) + "-" + data[index + 4].ToString("X") + "-" + data[index + 3].ToString("X") + " " + data[index + 2].ToString("X") + ":" + data[index + 1].ToString("X") + ":" + data[index].ToString("");

                index += 6;
                string ady = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));
                index += 2;
                string bdy = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));
                index += 2;
                string cdy = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));

                index += 8;
                string adl = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));
                index += 2;
                string bdl = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));
                index += 2;
                string cdl = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 2));

                try
                {
                    model.date = DateTime.Parse(sjrq).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch
                {
                    model.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                model.xdy1 = Convert.ToDecimal(int.Parse(ady) / 10.0);
                model.xdy2 = Convert.ToDecimal(int.Parse(bdy) / 10.0);
                model.xdy3 = Convert.ToDecimal(int.Parse(cdy) / 10.0);

                model.dl1 = Convert.ToDecimal(int.Parse(adl) / 100.00);
                model.dl2 = Convert.ToDecimal(int.Parse(bdl) / 100.00);
                model.dl3 = Convert.ToDecimal(int.Parse(cdl) / 100.00);
                TxtLogHelper.WriteLog("解析命令:A电压" + ady + " B电压" + bdy + " C电压 " + cdy + " A电流" + adl + " B电流" + bdl + " C电流" + cdl, "TCP",1);
                index = 12 + int.Parse(zjs) + 14;
                if (data.Length > (index + 68))
                {
                    zjs = data[index + 2].ToString();
                    index += 3;
                    string zdn = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 4));
                    index += 20;
                    string adn = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 4));
                    index += 4;
                    index += 16;
                    string bdn = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 4));
                    index += 4;
                    index += 16;
                    string cdn = ByteHelper.HexToTen(ByteHelper.ByteToString(data, index, 4));


                    model.zydl = Convert.ToDecimal(int.Parse(zdn) / 100.00);
                    model.aydl = Convert.ToDecimal(int.Parse(adn) / 100.00);
                    model.bydl = Convert.ToDecimal(int.Parse(bdn) / 100.00);
                    model.cydl = Convert.ToDecimal(int.Parse(cdn) / 100.00);
                    TxtLogHelper.WriteLog("解析命令: A电能" + adn + " B电能" + bdn + " C电能" + cdn + " 总电能" + zdn, "TCP",1);
                }

                //dlrep.Insert(model);
            }
            TxtLogHelper.WriteLog("设备数据域上传回复命令内容：" + "7b 7b 91 7e ec 7d 7d", "TCP",1);
            ConsoleWriteLine("设备数据域上传回复命令内容：7b 7b 91 7e ec 7d 7d",1);
            return ByteHelper.StringToByte("7b 7b 91 7e ec 7d 7d");//固定值
        }
        /// <summary>
        /// 设备注册命令
        /// </summary>
        private byte[] Register(byte[] data)
        {
            //string xlh = ByteHelper.HexToTen(ByteHelper.ByteToString(data, 3, 20, "D5"));
            //string kh = ByteHelper.HexToTen(ByteHelper.ByteToString(data, 23, 30));
            string xhqd = ByteHelper.HexToTen(ByteHelper.ByteToString(data, 53, 1));
            string gjbb1 = ByteHelper.HexToTen(ByteHelper.ByteToString(data, 54, 2));
            string gjbb2 = ByteHelper.HexToTen(ByteHelper.ByteToString(data, 56, 2));
            string gjbb3 = ByteHelper.HexToTen(ByteHelper.ByteToString(data, 58, 2));
            string sjjg = ByteHelper.HexToTen(ByteHelper.ByteToString(data, 60, 1));

            //AShebeiRepository sbrep = new AShebeiRepository();
            //a_shebei sbmodel = sbrep.SearchModelByXlh(Hex2Ten(xlh));
            //if (sbmodel == null)
            //{
            //    sbmodel = new a_shebei();
            //    sbmodel.xlh = Hex2Ten(xlh);
            //    sbmodel.kh = Hex2Ten(kh);
            //    sbmodel.xhqd = int.Parse(Hex2Ten(xhqd));
            //    sbmodel.gjbb1 = Hex2Ten(gjbb1);
            //    sbmodel.gjbb2 = Hex2Ten(gjbb2);
            //    sbmodel.gjbb3 = Hex2Ten(gjbb3);
            //    sbmodel.scjg = int.Parse(Hex2Ten(sjjg));
            //    sbmodel.cjsj = DateTime.Now.ToString("yyyy-MM-dd");
            //    sbmodel.ipdz = clientipe.Address.ToString() + ":" + clientipe.Port.ToString();
            //    int num = sbrep.Insert(sbmodel);
            //    //TxtLogHelper.WriteLog("设备注册添加数据返回值：" + num, 0);
            //}
            //else
            //{
            //    sbmodel.xlh = Hex2Ten(xlh);
            //    sbmodel.kh = Hex2Ten(kh);
            //    sbmodel.xhqd = int.Parse(Hex2Ten(xhqd));
            //    sbmodel.gjbb1 = Hex2Ten(gjbb1);
            //    sbmodel.gjbb2 = Hex2Ten(gjbb2);
            //    sbmodel.gjbb3 = Hex2Ten(gjbb3);
            //    sbmodel.scjg = int.Parse(Hex2Ten(sjjg));
            //    sbmodel.xgsj = DateTime.Now.ToString("yyyy-MM-dd");
            //    sbmodel.ipdz = clientipe.Address.ToString() + ":" + clientipe.Port.ToString();
            //    if (sbrep.Edit(sbmodel))
            //    {
            //        //TxtLogHelper.WriteLog("设备注册修改数据成功", 0);
            //    }
            //    else
            //    {
            //        //TxtLogHelper.WriteLog("设备注册修改数据失败", 0);
            //    }
            //}
            TxtLogHelper.WriteLog("设备注册回复命令内容：" + "7b 7b 84 bf 23 7d 7d", "TCP");
            ConsoleWriteLine("设备注册回复命令内容：7b 7b 84 bf 23 7d 7d",1);
            return ByteHelper.StringToByte("7b 7b 84 bf 23 7d 7d");//固定值
        }
        /// <summary>
        /// 设备上传报警设定值
        /// </summary>
        private byte[] AlarmUpload(byte[] data)
        {
            TxtLogHelper.WriteLog("设备上传报警设定值回复命令内容：" + "7b 7b 89 7e e6 7d 7d", "TCP");
            ConsoleWriteLine("设备上传报警设定值回复命令内容：7b 7b 89 7e e6 7d 7d",1);
            return ByteHelper.StringToByte("7b 7b 89 7e e6 7d 7d");//固定值
        }
        /// <summary>
        /// 设备回复服务器消息
        /// </summary>
        private void ReplyMessage(byte[] data)
        {
            switch (data[2])
            {
                case 0xA3:
                    #region  服务器下发固件更新信息回复
                    TxtLogHelper.WriteLog("服务器下发固件更新信息指令已收到回复", "TCP");
                    #endregion
                    break;
                case 0x90:
                    #region  Modbus数据透传回复
                    TxtLogHelper.WriteLog("Modbus数据透传回复已收到回复", "TCP");
                    #endregion
                    break;
                case 0x88:
                    #region  设置IP端口回复
                    TxtLogHelper.WriteLog("设置IP端口回复已收到回复", "TCP");
                    #endregion
                    break;
                case 0x87:
                    #region  重启设备回复
                    TxtLogHelper.WriteLog("重启设备回复已收到回复", "TCP");
                    #endregion
                    break;
                case 0x82:
                    #region  设备参数下发回复
                    TxtLogHelper.WriteLog("设备参数下发回复已收到回复", "TCP");
                    #endregion
                    break;
            }
        }
        #endregion
        #endregion

        #region 公共方法
        /// <summary>
        /// 根据key查询对应的客户端连接id
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>不存在返回0</returns>
        private int GetConnectId(string key)
        {
            int connectId = 0;
            if (clientList != null && clientList.TryGetValue(key, out connectId))
            {
                if (server.ClientList!=null&server.ClientList.ContainsKey(connectId))
                {
                    return connectId;
                }
                else
                {
                    clientList.TryRemove(key, out connectId);
                }
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="connectId">客户端连接id</param>
        /// <returns></returns>
        private bool AddConnectId(string key, int connectId)
        {
            if (clientList == null)
                clientList = new ConcurrentDictionary<string, int>();
            return clientList.TryAdd(key, connectId);
        }
        /// <summary>
        /// 根据自定义Key发送数据
        /// </summary>
        /// <param name="Key">Key</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        public bool TcpSend(string Key, byte[] data, int offset = 0)
        {
            int connectId = GetConnectId(Key);
            if (connectId > 0)
            {
                server.Send(connectId, data, offset, data.Length);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 控制台打印
        /// </summary>
        /// <param name="content">打印内容</param>
        /// <param name="_type">文字颜色 0 白色 1 蓝色 2 绿色 3 红色</param>
        public void ConsoleWriteLine(string content, int _type=0)
        {
            switch (_type)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(content);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(content);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(content);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    Console.WriteLine(content);
                    break;
            }
        }
        #endregion
    }
}
