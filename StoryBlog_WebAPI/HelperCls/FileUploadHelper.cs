using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace StoryBlog_WebAPI.HelperCls
{

    /// <summary>
    /// webuploader文件上传处理类
    /// </summary>
    public class FileUploadHelper
    {
        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckFileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string UploadHandler(HttpContext context, string svaePath)
        {
            string result = "";
            try
            {
                context.Response.ContentType = "text/plain";
                //string fileName = context.Request["fileName"];
                //string rootPath = context.Server.MapPath("~/RambleFile/" + fileName);
                //if (CheckFileExists(context.Server.MapPath("~/RambleFile/" + fileName)))
                //{
                //    return "{\"result\":false,\"msg\":\"服务器存在相同文件\"}";
                //}

                //如果进行了分片
                if (context.Request.Form.AllKeys.Any(m => m == "chunk"))
                {
                    //取得chunk和chunks
                    int chunk = Convert.ToInt32(context.Request.Form["chunk"]); //当前分片在上传分片中的顺序（从0开始）
                    int chunks = Convert.ToInt32(context.Request.Form["chunks"]);   //总分片数
                    string folder = svaePath + context.Request["guid"] + "/";   //根据GUID创建GUID命名的临时文件夹临时存放文件的路径
                    string path = folder + chunk;
                    //建立临时传输文件夹
                    if (!Directory.Exists(Path.GetDirectoryName(folder)))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    FileStream addFile = new FileStream(path, FileMode.Append, FileAccess.Write);
                    BinaryWriter AddWriter = new BinaryWriter(addFile);
                    //获得上传的分片数据流
                    HttpPostedFile file = context.Request.Files[0];
                    Stream stream = file.InputStream;

                    BinaryReader TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                    result = "{\"result\" : true,\"chunked\" : true}";
                }
                else    //没有分片直接保存
                {
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + context.Request.Files[0].FileName;
                    context.Request.Files[0].SaveAs(context.Server.MapPath("~/RambleFile/" + fileName));
                    result = "{\"result\" : true,\"chunked\" : false, \"fileName\" : \"" + fileName + "\"}";
                }
            }
            catch (Exception ex)
            {
                result = "{\"result\" : false, \"msg\" : \"" + ex.Message + "\"}";
            }
            return result;
        }

        /// <summary>
        /// 合并上传文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool MergeUploadFile(HttpContext context, string savePath, out string fileName)
        {
            context.Response.ContentType = "text/plain";
            string guid = context.Request["guid"];
            fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + context.Request["fileName"]; //保存的文件名
            string sourcePath = Path.Combine(savePath, guid + "/"); //源数据文件夹
            string targetPath = Path.Combine(savePath, fileName);   //合并后的文件

            DirectoryInfo dicInfo = new DirectoryInfo(sourcePath);
            if (Directory.Exists(Path.GetDirectoryName(sourcePath)))
            {
                FileInfo[] files = dicInfo.GetFiles();
                foreach (FileInfo file in files.OrderBy(f => int.Parse(f.Name)))
                {
                    FileStream addFile = new FileStream(targetPath, FileMode.Append, FileAccess.Write);
                    BinaryWriter AddWriter = new BinaryWriter(addFile);

                    //获得上传的分片数据流
                    Stream stream = file.Open(FileMode.Open);
                    BinaryReader TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                }
                DeleteFolder(sourcePath);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dir"></param>
        private static void DeleteFolder(string strPath)
        {
            //删除这个目录下的所有子目录
            if (Directory.GetDirectories(strPath).Length > 0)
            {
                foreach (string fl in Directory.GetDirectories(strPath))
                {
                    Directory.Delete(fl, true);
                }
            }
            //删除这个目录下的所有文件
            if (Directory.GetFiles(strPath).Length > 0)
            {
                foreach (string f in Directory.GetFiles(strPath))
                {
                    System.IO.File.Delete(f);
                }
            }
            Directory.Delete(strPath, true);
        }
    }
}