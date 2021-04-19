using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Zeiot.Core
{
    /// <summary>
    /// 加密字符串辅助类
    /// </summary>
    public class Encrypt
    {
        /// <summary>
        /// MD5 hash加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string MD5(string s)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var result = BitConverter.ToString(md5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(s.Trim())));
            return result.Replace("-", "");
        }
        private const string CIV = "kXwL7X2AfgM=";      //初始化向量
        private const string CKEY = "FwGQWRRgKCI=";     //密钥
        // 加密
        public static string EncryptNew(string value)
        {
            try
            {
                SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();                // 对象算法对象

                ICryptoTransform ct;        // 基本加密转换运算类
                MemoryStream ms;            // 存储区为内存的流对象
                CryptoStream cs;            // 将数据链接到加密转换的流对象     
                byte[] byt;

                // 根据密钥和初始化向量创建基本加密转换运算对象
                ct = mCSP.CreateEncryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV));
                // 将要加密的字符串转换成byte数组
                byt = Encoding.UTF8.GetBytes(value);
                // 对数据进行加密处理
                ms = new MemoryStream();
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                cs.Close();

                // 返回相密的结果
                return Base64.ToBase64String(ms.ToArray());
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        // 解密
        public static string Decrypt(string value)
        {
            SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();                // 对象算法对象
            ICryptoTransform ct;        // 基本加密转换运算类
            MemoryStream ms;            // 存储区为内存的流对象
            CryptoStream cs;            // 将数据链接到加密转换的流对象     
            byte[] byt;

            try
            {
                // 根据密钥和初始化向量创建基本加密转换运算对象
                ct = mCSP.CreateDecryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV));
                // 将要解密的字符串转换成byte数组
                byt = Base64.FromBase64String(value);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();

                cs.Close();
            }
            catch (Exception ex)
            {
                return "";
            }

            return Encoding.UTF8.GetString(ms.ToArray());

        }


        #region DES加密解密
        //默认密钥向量
        private static byte[] keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string Encode(string encryptString, string encryptKey = "www.GMS.com")
        {
            encryptKey = encryptKey.Substring(0, 8);
            encryptKey = encryptKey.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());

        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string Decode(string decryptString, string decryptKey = "www.GMS.com")
        {
            try
            {
                decryptKey = decryptKey.Substring(0, 8);
                decryptKey = decryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "";
            }

        }
        #endregion
    }
}
