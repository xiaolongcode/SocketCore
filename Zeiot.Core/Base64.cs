using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Core
{
    public class Base64
    {

        /// <summary>
        /// 普通字符串转换为base64字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringToBase64(string s)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(s);
            return ToBase64String(b);

        }
        /// <summary>
        /// base64字符串转换为普通字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64ToString(string s)
        {
            byte[] c = FromBase64String(s);
            return System.Text.Encoding.Default.GetString(c);
        }


        /// <summary>
        /// 将图片流字节数组转换为base64字符串
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ConvertByteToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 将图像转换为base64字符串
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ConvertImageToBase64(Image file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.Save(memoryStream, file.RawFormat);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }
        /// <summary>
        /// base64字符串转换为图像
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Image ConvertBase64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }


        #region 自定义base64加码解码 去除部分特殊符号
        private static readonly string base64Table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";
        private static readonly int[] base64Index = new int[]
        {
             -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
             -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
             -1,63,-1,-1,52,53,54,55,56,57,58,59,60,61,-1,-1,-1,-1,-1,-1,-1,0,1,
             2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,-1,
             -1,-1,-1,62,-1,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,
             43,44,45,46,47,48,49,50,51,-1,-1,-1,-1,-1,-1
        };
        /// <summary>
        /// 自定义base64加码解码 去除部分特殊符号
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(string inData)
        {
            int inDataLength = inData.Length;
            int lengthmod4 = inDataLength % 4;
            int calcLength = (inDataLength - lengthmod4);
            byte[] outData = new byte[inDataLength / 4 * 3 + 3];
            int j = 0;
            int i;
            int num1, num2, num3, num4;

            for (i = 0; i < calcLength; i += 4, j += 3)
            {
                num1 = base64Index[inData[i]];
                num2 = base64Index[inData[i + 1]];
                num3 = base64Index[inData[i + 2]];
                num4 = base64Index[inData[i + 3]];

                outData[j] = (byte)((num1 << 2) | (num2 >> 4));
                outData[j + 1] = (byte)(((num2 << 4) & 0xf0) | (num3 >> 2));
                outData[j + 2] = (byte)(((num3 << 6) & 0xc0) | (num4 & 0x3f));
            }
            i = calcLength;
            switch (lengthmod4)
            {
                case 3:
                    num1 = base64Index[inData[i]];
                    num2 = base64Index[inData[i + 1]];
                    num3 = base64Index[inData[i + 2]];

                    outData[j] = (byte)((num1 << 2) | (num2 >> 4));
                    outData[j + 1] = (byte)(((num2 << 4) & 0xf0) | (num3 >> 2));
                    j += 2;
                    break;
                case 2:
                    num1 = base64Index[inData[i]];
                    num2 = base64Index[inData[i + 1]];

                    outData[j] = (byte)((num1 << 2) | (num2 >> 4));
                    j += 1;
                    break;
            }
            Array.Resize(ref outData, j);
            return outData;
        }
        /// <summary>
        /// 自定义base64加码解码 去除部分特殊符号
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        public static string ToBase64String(byte[] inData)
        {
            int inDataLength = inData.Length;
            int outDataLength = (int)(inDataLength / 3 * 4) + 4;
            char[] outData = new char[outDataLength];

            int lengthmod3 = inDataLength % 3;
            int calcLength = (inDataLength - lengthmod3);
            int j = 0;
            int i;

            for (i = 0; i < calcLength; i += 3, j += 4)
            {
                outData[j] = base64Table[inData[i] >> 2];
                outData[j + 1] = base64Table[((inData[i] & 0x03) << 4) | (inData[i + 1] >> 4)];
                outData[j + 2] = base64Table[((inData[i + 1] & 0x0f) << 2) | (inData[i + 2] >> 6)];
                outData[j + 3] = base64Table[(inData[i + 2] & 0x3f)];
            }

            i = calcLength;
            switch (lengthmod3)
            {
                case 2:
                    outData[j] = base64Table[inData[i] >> 2];
                    outData[j + 1] = base64Table[((inData[i] & 0x03) << 4) | (inData[i + 1] >> 4)];
                    outData[j + 2] = base64Table[(inData[i + 1] & 0x0f) << 2];
                    j += 3;
                    break;
                case 1:
                    outData[j] = base64Table[inData[i] >> 2];
                    outData[j + 1] = base64Table[(inData[i] & 0x03) << 4];
                    j += 2;
                    break;
            }
            return new string(outData, 0, j);
        }
        #endregion


    }
}
