using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zeiot.Core
{
    public class IPHelper
    {

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetHostIP()
        {
            string ip = "";
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null) // using proxy 
            {
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString(); // Return real client IP. 
            }
            else// not using proxy or can't get the Client IP 
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP. 
            }
            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }
            return ip;
        }

        /// <summary>
        /// 获取客户端所在物理位置（百度地图）
        /// </summary>
        /// <returns></returns>
        public static string GetHostAddress(string ip)
        {
            string address = "";
            if (!string.IsNullOrEmpty(ip))
            {
                try
                {
                    string u = string.Format("http://api.map.baidu.com/location/ip?ip={0}&ak={1}&coor=bd09ll", ip, "NDLFIeDEy1wso902zM0Ge5sGVVkuNeqb");//百度ak秘钥
                    string re = SendRequest(u, Encoding.UTF8);
                    //string ee = UnicodeToString(re);
                    JObject Obj = JObject.Parse(re);
                    string status = Obj["status"].ToString();
                    if (status == "0")
                    {
                        var loca = Obj["content"]["address_detail"];
                        address = loca["province"].ToString() + loca["city"].ToString() + loca["district"].ToString() + loca["street"].ToString() + loca["street_number"].ToString();
                    }
                }
                catch (Exception ex)
                {

                    //throw;
                }

            }

            return address;
        }

        /// <summary>
        /// Unicode字符串转为正常字符串
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns></returns>
        public static string UnicodeToString(string srcText)
        {
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        }


        /// <summary>   
        /// Get方式获取url地址输出内容   
        /// </summary> /// <param name="url">url</param>   
        /// <param name="encoding">返回内容编码方式，例如：Encoding.UTF8</param>   
        public static string SendRequest(string url, Encoding encoding)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), encoding);
            string str = sr.ReadToEnd();
            return str;
        }

        /// <summary>
        /// 判断指定ip 端口 是否可以连接
        /// </summary>
        /// <returns></returns>
        public static bool IpPoreOpen(string _ip, string port)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(_ip);
                IPEndPoint point = new IPEndPoint(ip, int.Parse(port));
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(point);
                return true;
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10061)
                {
                    //Console.WriteLine(e.Message);
                }
            }
            return false;
        }
    }
}
