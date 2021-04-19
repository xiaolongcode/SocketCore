using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;

namespace Zeiot.Core
{


    /// <summary>
    /// 二维码生成
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 根据指定内容获取二维码
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns>返回二维码图片</returns>
        private static Bitmap GetQRCodeBmp(string content)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            Bitmap bmp = qrCodeEncoder.Encode(content);
            return bmp;
        }
        /// <summary>
        /// 根据指定内容生成二维码，并返回Base64
        /// </summary>
        ///  <param name="content">内容</param>
        /// <returns></returns>
        public static string QRCodeByBase64(string content)
        {
           return Base64.ConvertByteToBase64(QRCodeByByte(content));
        }
        /// <summary>
        /// 根据指定内容生成二维码，并返回字节流
        /// </summary>
        ///  <param name="content">内容</param>
        /// <returns></returns>
        public static byte[] QRCodeByByte(string content)
        {
            Bitmap bitmap = GetQRCodeBmp(content);
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }
        /// <summary>
        /// 根据指定内容生成二维码，并返回字文件流
        /// </summary>
        ///  <param name="content">内容</param>
        /// <returns></returns>
        public static MemoryStream QRCodeByStream(string content)
        {
            Bitmap bitmap = GetQRCodeBmp(content);
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                return stream;
            }
        }
        /// <summary>
        /// 根据指定内容生成二维码，并保存到对应路径
        /// </summary>
        /// <param name="imgpath">图片路径</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static bool SaveImg(string imgpath, string content)
        {
            try
            {
                GetQRCodeBmp(content).Save(imgpath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }


}
