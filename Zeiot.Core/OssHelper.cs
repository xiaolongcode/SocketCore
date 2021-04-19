using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Web;
namespace Zeiot.Core
{
    public class OssHelper
    {
        OssClient client = null;
        static string oss_endPoint = ConfigurationManager.AppSettings["OSSPath"];
        static string oss_accessId = ConfigurationManager.AppSettings["OSSKey"];
        static string oss_accessKeySeCret = ConfigurationManager.AppSettings["OSSSecret"];
        static string bucketName = ConfigurationManager.AppSettings["OSSBucket"];

        static ObjectMetadata meta = new ObjectMetadata();
        static string _ossPath = ConfigurationManager.AppSettings["aliyunOSS"];
        static string _yxwfileossPath = ConfigurationManager.AppSettings["yxwfileOSS"];
        public OssHelper()
        {
            client = new OssClient(oss_endPoint, oss_accessId, oss_accessKeySeCret);
        }
        /// <summary>
        /// 获取阿里云根路径
        /// </summary>
        /// <returns></returns>
        public static string GetOssPath()
        {
            if (string.IsNullOrEmpty(_ossPath))
            {
                _ossPath = ConfigurationManager.AppSettings["aliyunOSS"];
            }

            return _ossPath;
        }

        /// <summary>
        /// 获取阿里云根路径 yxwfileOSS
        /// </summary>
        /// <returns></returns>
        public static string GetyxwfileOSS()
        {
            if (string.IsNullOrEmpty(_yxwfileossPath))
            {
                _yxwfileossPath = ConfigurationManager.AppSettings["yxwfileOSS"];
            }

            return _yxwfileossPath;
        }
        /// <summary>
        /// 根据扩展名创建meta文件类型
        /// </summary>
        /// <param name="extension">文件的扩展名</param>
        /// <returns></returns>
        public static string ContentType(string extension)
        {
            extension = extension.ToLower();
            string contentType = "image/jpeg";
            if (extension == "txt")
            {
                contentType = "text/plain";
            }
            else if (extension == "htm" || extension == "html")
            {
                contentType = "text/html";
            }
            else if (extension == "xml")
            {
                contentType = "text/xml";
            }
            else if (extension == "json")
            {
                contentType = "text/json";
            }
            else if (extension == "md")
            {
                contentType = "text/md";
            }
            else if (extension == "jpg" || extension == "jpeg")
            {
                contentType = "image/jpeg";
            }
            else if (extension == "gif")
            {
                contentType = "image/gif";
            }
            else if (extension == "png")
            {
                contentType = "image/png";
            }
            else if (extension == "js")
            {
                contentType = "application/x-javascript";
            }
            else if (extension == "map")
            {
                contentType = "application/x-javascript";
            }
            else if (extension == "css")
            {
                contentType = "text/css";
            }
            else if (extension == "wmv")
            {
                contentType = "video/x-ms-wmv";
            }
            else if (extension == "mp4")
            {
                contentType = "video/mpeg4";
            }
            else if (extension == "avi")
            {
                contentType = "video/avi";
            }
            else if (extension == "flv")
            {
                contentType = "video/flv";
            }
            else if (extension == "pdf")
            {
                contentType = "	application/pdf";
            }
            else if (extension == "doc")
            {
                contentType = "	application/msword";
            }
            else if (extension == "xls")
            {
                contentType = "	application/x-xls";
            }
            else if (extension == "docx")
            {
                contentType = "	application/msword";
            }
            else if (extension == "xlsx")
            {
                contentType = "	application/x-xls";
            }
            else if (extension == "img")
            {
                contentType = "	application/x-img";
            }
            else if (extension == "bmp")
            {
                contentType = "	application/x-bmp";
            }
            else if (extension == "dot")
            {
                contentType = "	application/msword";
            }
            else if (extension == "jpe")
            {
                contentType = " image/jpeg";
            }
            else if (extension == "mp4")
            {
                contentType = " video/mp4";
            }
            else if (extension == "swf")
            {
                contentType = " application/x-shockwave-flash";
            }
            else if (extension == "xml")
            {
                contentType = " text/xml";
            }
            else if (extension == "apk")
            {
                contentType = " application/vnd.android.package-archive";
            }
            else if (extension == "xhtml")
            {
                contentType = " text/html";
            }
            else if (extension == "mfp")
            {
                contentType = " application/x-shockwave-flash";
            }
            return contentType;
        }

        /// <summary>
        /// 创建新的Bucket
        /// </summary>
        /// <param name="bucketName">oss磁盘名称</param>
        public void CreateBucket(string bucketName)
        {
            ////初始化 OSSClient
            //OssClient ossClient = new OssClient(endPoint, accessKeyId, accessKeySecret);
            // 新建一个Bucket
            client.CreateBucket(bucketName);
            // 设置bucket权限
            client.SetBucketAcl(bucketName, CannedAccessControlList.PublicRead);
        }

        /// <summary>
        /// 获取当前目录下，说有的文件
        /// </summary>
        /// <param name="bucketName">oss磁盘名称</param>
        /// <returns></returns>
        public List<OssObjectSummary> ListObject(string bucketName)
        {
            try
            {
                List<OssObjectSummary> lps = new List<OssObjectSummary>();
                ObjectListing result = null;
                string nextMarker = string.Empty;
                do
                {
                    var listObjectsRequest = new ListObjectsRequest(bucketName)
                    {
                        Marker = nextMarker,
                        MaxKeys = 1000
                    };
                    result = client.ListObjects(listObjectsRequest);
                    // Console.WriteLine("System.IO.File:");
                    foreach (var summary in result.ObjectSummaries)
                    {
                        lps.Add(summary);
                        // Console.WriteLine("Name:{0}", summary.Key);
                    }
                    nextMarker = result.NextMarker;
                } while (result.IsTruncated);
                return lps;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("List object failed. {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取当前目录的子目录
        /// </summary>
        /// <param name="bucketName">oss磁盘名称</param>
        /// <param name="ossDirectory">文件夹路径</param>
        /// <returns></returns>
        public List<OssObjectSummary> ListObjectByDir(string bucketName, string ossDirectory = "")
        {
            List<OssObjectSummary> lps = new List<OssObjectSummary>();
            string oss_prefix = ossDirectory; // Properties.Settings.Default.osspath : ossDirectory;
            ObjectListing result = null;
            string nextMarker = string.Empty;
            do
            {
                var listObjectsRequest = new ListObjectsRequest(bucketName)
                {
                    Marker = nextMarker,
                    MaxKeys = 1000,
                    Prefix = oss_prefix
                };
                result = client.ListObjects(listObjectsRequest);
                Console.WriteLine("System.IO.File:");
                foreach (var summary in result.ObjectSummaries)
                {
                    lps.Add(summary);
                    Console.WriteLine("Name:{0}", summary.Key);
                }
                nextMarker = result.NextMarker;
            } while (result.IsTruncated);
            return lps;
        }

        /// <summary>
        ///     获取指定目录的文件和子目录
        /// </summary>
        /// <param name="bucketName">oss磁盘名称</param>
        /// <param name="ossDirectory">文件路径</param>
        /// <returns></returns>
        public ObjectListing ListObjects(string bucketName, string ossDirectory)
        {
            try
            {
                string oss_prefix = ossDirectory; //ossDirectory == "" ? Properties.Settings.Default.osspath : ossDirectory;
                var listObjectsRequest = new ListObjectsRequest(bucketName)
                {
                    Prefix = oss_prefix,
                    Delimiter = "/"
                };
                var result = client.ListObjects(listObjectsRequest);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("List object failed. {0}", ex.Message);
                return null;

            }
        }

        /// <summary>
        /// 文件上传到oss
        /// </summary>
        /// <param name="DownloadPath">文件的路径（oss和本地路径必须一致）</param>
        /// <returns></returns>
        public bool PutObject(string DownloadPath)
        {
            string FilePath = HttpContext.Current.Server.MapPath("/") + DownloadPath;
            return PutObject(FilePath, DownloadPath);
        }

        /// <summary>
        /// 文件上传到oss
        /// </summary>
        /// <param name="FilePath">要上传的文件的本地路径</param>
        /// <param name="DownloadPath">上传到oss的目录</param>
        /// <returns></returns>
        public bool PutObject(string FilePath, string DownloadPath)
        {
            Stream fileStream = null;
            try
            {

                DirectoryInfo fil = new DirectoryInfo(FilePath);
                string newpath = fil.FullName.Replace(fil.Name, "");
                newpath = newpath + "ls" + DateTime.Now.ToString("ddHHmmssfff") + fil.Name;
                if (System.IO.File.Exists(newpath))
                    System.IO.File.Exists(newpath);
                System.IO.File.Copy(FilePath, newpath);

                fileStream = System.IO.File.Open(newpath, FileMode.Open, FileAccess.Read);

                string refmsg = PutObject(fileStream, DownloadPath);
                if (refmsg == "OK")
                {
                    //是否需要删除文件
                    fileStream.Dispose();
                    fileStream.Close();
                    System.IO.File.Delete(newpath);
                    System.IO.File.Delete(FilePath);
                    return true;
                }
                else
                {
                    fileStream.Dispose();
                    fileStream.Close();
                    System.IO.File.Delete(newpath);
                    System.IO.File.Delete(FilePath);
                    return false;

                }

            }
            catch (Exception ex)
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream.Close();
                }
                return false;
            }
        }
        /// <summary>
        /// 将文件上传到oss
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public string PutObject(Stream fileStream, string DownloadPath)
        {
            try
            {
                DownloadPath = DownloadPath.Replace("\\", "/");
                meta.CacheControl = "no-cache";
                meta.ContentEncoding = "utf-8";
                meta.ContentLength = fileStream.Length;
                if (!client.DoesBucketExist(bucketName))
                {
                    CreateBucket(bucketName);
                }
                meta.ContentType = ContentType(Path.GetExtension(DownloadPath).Substring(1));
                //var result = client.PutObject("wangjie-img-hz", imgname, imgStream, meta);
                var result = client.PutObject(bucketName, DownloadPath, fileStream, meta);
         
                return result.HttpStatusCode.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="DownloadPath">阿里云文件目录</param>
        /// <param name="fileSavePath">本地保存文件目录</param>
        public bool GetObject(string DownloadPath, string fileSavePath)
        {
            FileStream fs = null;
            Stream requestStream = null;
            try
            {
                Uri u = new Uri(GetOssPath() + DownloadPath);

                DirectoryInfo fil = new DirectoryInfo(fileSavePath);
                string path = fil.FullName.Replace(fil.Name, "");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (System.IO.File.Exists(fileSavePath))
                    System.IO.File.Delete(fileSavePath);

                OssObject obj = client.GetObject(u);
                using (requestStream = obj.Content)
                {
                    byte[] buf = new byte[1024];
                    fs = System.IO.File.Open(fileSavePath, FileMode.OpenOrCreate);
                    var len = 0;
                    while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Dispose();
                    fs.Close();
                    requestStream.Dispose();
                    requestStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Dispose();
                    fs.Close();
                }
                if (requestStream != null)
                {
                    requestStream.Dispose();
                    requestStream.Close();
                }
                return false;
            }
        }
        /// <summary>
        /// 删除oss文件
        /// </summary>
        /// <param name="DownloadPath">文件路径</param>
        /// <returns></returns>
        public bool DeleteObject(string DownloadPath)
        {
            try
            {
                client.DeleteObject(bucketName, DownloadPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
