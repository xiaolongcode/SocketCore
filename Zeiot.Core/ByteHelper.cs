using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Core
{
    /// <summary>
    /// byte数据操作类
    /// </summary>
    public class ByteHelper
    {
        /// <summary>
        /// byte[]格式转换为指定进制字符串(带空格)
        /// </summary>
        /// <param name="data">byte[] 数据</param>
        /// <param name="tobase">字符串的进制基数 默认为16进制</param>
        /// <returns>字符串 每个数据之间以空格分隔</returns>
        public static string ByteToString(byte[] data, int tobase = 16)
        {
            try
            {
                string ReStr = " ";
                for (int i = 0; i < data.Length; i++)
                {
                    ReStr += Convert.ToString(data[i], tobase).PadLeft(2, '0') + " ";
                }
                return ReStr;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary> 
        /// 字符串转节数数组 byte[]
        /// </summary> 
        /// <param name="hexString">字符串</param> 
        /// <param name="frombase">byte的进制基数 默认为16进制</param>
        /// <returns>byte[] 数据</returns> 
        public static byte[] StringToByte(string hexString, int frombase = 16)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), frombase);
            return returnBytes;
        }
        /// <summary>
        /// 十六进制字符串转换到十进制字符串
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string HexToTen(string hex)
        {
            try
            {
                return int.Parse(hex, System.Globalization.NumberStyles.AllowHexSpecifier).ToString();
            }
            catch
            {
                return long.Parse(hex, System.Globalization.NumberStyles.AllowHexSpecifier).ToString();
            }
        }
        /// <summary>
        /// 十进制字符串转十六进制字符串
        /// </summary>
        /// <param name="字符串">每个字节之间用，号分割</param>
        /// <returns></returns>
        public static byte[] TenToHex(string hexString)
        {
            if (hexString.Contains(","))
            {
                string[] strs = hexString.Split(',');
                byte[] returnBytes = new byte[strs.Length];
                for (int i = 0; i < strs.Length; i++)
                {
                    returnBytes[i] = Convert.ToByte(strs[i], 16);
                }
                return returnBytes;
            }
            else
            {
                byte[] returnBytes = new byte[1];
                returnBytes[0] = Convert.ToByte(hexString, 16);
                return returnBytes;
            }
        }
        /// <summary>
        /// 根据16进制字符串返回校验码字符串(逗分割)
        /// </summary>
        /// <param name="hexString">16进制字符串</param>
        /// <returns></returns>
        public static string CRC16_String(byte[] data)
        {
            byte CRC16Lo;
            byte CRC16Hi;   //CRC寄存器 
            byte CL; byte CH;       //多项式码&HA001 
            byte SaveHi; byte SaveLo;
            byte[] tmpData;
            int Flag;
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;
            CL = 0x01;
            CH = 0xA0;
            tmpData = data;
            for (int i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位 
                    CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位 
                    if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1 
                    {
                        CRC16Lo = (byte)(CRC16Lo | 0x80);   //则低位字节右移后前面补1 
                    }             //否则自动补0 
                    if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或 
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }
            byte[] ReturnData = new byte[2];
            ReturnData[1] = CRC16Hi;       //CRC高位 
            ReturnData[0] = CRC16Lo;       //CRC低位 
            return ByteToString(ReturnData).Trim(' ').Replace(' ', ',');
        }
        /// <summary>
        /// 根据起始位置和数量 返回指定范围的byte数据字符串(不带空格)
        /// </summary>
        /// <param name="data">byte[] 数据</param>
        /// <param name="benindex">起始位置</param>
        /// <param name="length">长度</param>
        /// <param name="tobase">字符串的进制基数 16进制为X 10进制为D5 默认为X</param>
        /// <returns></returns>
        public static string ByteToString(byte[] data, int benindex, int length, string tobase = "X")
        {
            string hexstring = "";
            for (int i = benindex; i < (benindex + length); i++)
            {
                string s= data[i].ToString(tobase);
                if (tobase == "X"&& s=="0")
                {
                    s = "00";
                }
                hexstring += s;
            }
            return hexstring;
        }

        /// <summary>
        /// 字符串反转
        /// </summary>
        /// <param name="value">要反转的字符串</param>
        /// <returns></returns>
        private static string ReverseString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            string result = "";
            int length = value.Length;
            for(int i = length-1; i >=0; i--)
            {
                result += value.Substring(i,1);
            }
            return result;
        }
    }
}
