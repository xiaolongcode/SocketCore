using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Core
{
    public static class ZipHelper
    {
        /// <summary>
        /// 功能：解压zip格式的文件。
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        /// <param name="err">出错信息</param>
        /// <returns>解压是否成功</returns>
        public static void UnZip(string zipFilePath, string unZipDir, string filename)
        {
            try
            {
                if (zipFilePath == string.Empty)
                {
                    throw new Exception("压缩文件不能为空！");
                }
                if (!File.Exists(zipFilePath))
                {
                    throw new System.IO.FileNotFoundException("压缩文件不存在！");
                }
                //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
                if (unZipDir == string.Empty)
                    unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
                if (!unZipDir.EndsWith("\\"))
                    unZipDir += "\\";
                if (!Directory.Exists(unZipDir))
                    Directory.CreateDirectory(unZipDir);

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        try
                        {
                            string directoryName = Path.GetDirectoryName(theEntry.Name);
                            string fileName = Path.GetFileName(theEntry.Name);
                            if (string.IsNullOrEmpty(directoryName))
                            {
                                directoryName = filename;
                                fileName = filename + "/" + fileName;
                            }
                            if (directoryName.Length > 0)
                            {
                                if (!directoryName.EndsWith("\\"))
                                    directoryName += "\\";
                                if (directoryName.Split('\\')[1] != String.Empty)
                                {
                                    directoryName = directoryName.Split('\\')[0] + "\\";
                                }
                                Directory.CreateDirectory(unZipDir + directoryName);

                                if (fileName.IndexOf("/") < 0)
                                {
                                    fileName = directoryName + "/" + fileName;
                                }
                            }


                            if (fileName != String.Empty && (fileName.Contains("/") || fileName.Contains(".")) && !fileName.EndsWith("/"))
                            {
                                using (FileStream streamWriter = File.Create(unZipDir + fileName))
                                {

                                    int size = 2048;
                                    byte[] data = new byte[2048];
                                    while (true)
                                    {
                                        size = s.Read(data, 0, data.Length);
                                        if (size > 0)
                                        {
                                            streamWriter.Write(data, 0, size);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        /// <summary>
        /// 功能：解压zip格式的文件。
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径</param>
        /// <param name="err">出错信息</param>
        /// <returns>解压是否成功</returns>
        public static void UnZip(string zipFilePath, string unZipDir)
        {
            try
            {
                if (zipFilePath == string.Empty)
                {
                    throw new Exception("压缩文件不能为空！");
                }
                if (!File.Exists(zipFilePath))
                {
                    throw new System.IO.FileNotFoundException("压缩文件不存在！");
                }
                //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
                if (unZipDir == string.Empty)
                    unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
                if (!unZipDir.EndsWith("\\"))
                    unZipDir += "\\";
                if (!Directory.Exists(unZipDir))
                    Directory.CreateDirectory(unZipDir);

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        try
                        {
                            string fileName = Path.GetFileName(theEntry.Name);
                            
                            if (fileName != String.Empty && (fileName.Contains("/") || fileName.Contains(".")) && !fileName.EndsWith("/"))
                            {
                                using (FileStream streamWriter = File.Create(unZipDir + fileName))
                                {

                                    int size = 2048;
                                    byte[] data = new byte[2048];
                                    while (true)
                                    {
                                        size = s.Read(data, 0, data.Length);
                                        if (size > 0)
                                        {
                                            streamWriter.Write(data, 0, size);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }
    }
}
