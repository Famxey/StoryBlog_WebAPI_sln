using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Newtonsoft.Json;
using StoryBlog_WebAPI.HelperCls;
using StoryBlog_WebAPI.Models;

namespace StoryBlog_WebAPI.Controllers
{
    public class PictureInfoesController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();


        [HttpPost]
        [Route(Version_Helper.versionNumber + "/picture_/update")]
        public async Task<IEnumerable<FlagHelper>> PostPictureClsByEditCls(string uAccount, string picDescribe, int ID)
        {
            List<FlagHelper> list = new List<FlagHelper>();
            FlagHelper fg = new FlagHelper();

            if (uAccount == null)
            {
                fg.Flag = false;
                list.Add(fg);
                return list;
            }

            try
            {
                PictureInfo pic = await db.PictureInfo.Where(p => p.ID == ID).FirstOrDefaultAsync();

                pic.picDescribe = picDescribe;

                await db.SaveChangesAsync();

                fg.Flag = true;
                list.Add(fg);
                return list;
            }
            catch (Exception)
            {
                fg.Flag = false;
                list.Add(fg);
                return list;
            }
        }

        [HttpPost]
        [Route(Version_Helper.versionNumber + "/picture_/delete")]
        public async Task<IEnumerable<FlagHelper>> PostPicturesByDelete(string uAccount, int ID)
        {

            try
            {

                PictureInfo pic = await db.PictureInfo.Where(a => a.ID == ID).FirstOrDefaultAsync();

                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
                if (pic == null)
                {
                    fg.Flag = false;
                    list.Add(fg);
                    return list;
                }
                if (pic.uAccount != uAccount)
                {
                    fg.Flag = false;
                    list.Add(fg);
                    return list;
                }


                db.PictureInfo.Remove(pic);

                await db.SaveChangesAsync();

                fg.Flag = true;
                list.Add(fg);
                return list;
            }
            catch (Exception)
            {
                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
                fg.Flag = false;
                list.Add(fg);
                return list;
            }

        }

        [HttpPost]
        [Route(Version_Helper.versionNumber + "/picture_/add")]
        public async Task<IEnumerable<FlagHelper>> PostPicturesByAdd([FromBody]Newtonsoft.Json.Linq.JObject datas)
        {
            dynamic result = datas;
            string uAccount = result.uAccount;
            int picClsID = Convert.ToInt32(result.picClsID);
            string imageStr = result.image;

            List<FlagHelper> list = new List<FlagHelper>();
            FlagHelper fg = new FlagHelper();

            if (uAccount == null)
            {
                fg.Flag = false;
                list.Add(fg);
                return list;
            }

            try
            {
                #region            
                var tempUploadFiles = "/Upload/PictureFiles/";
                var newFileName = "";
                string filePath = "";
                string extname = "";
                string returnUrl = "";

                extname = imageStr.Split(';')[0].Replace("data:image/", ".");

                newFileName = Guid.NewGuid().ToString().Substring(0, 6) + uAccount + extname;
                string newFilePath = DateTime.Now.ToString("yyyy-MM-dd") + "/";

                if (!Directory.Exists(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath))
                {
                    Directory.CreateDirectory(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath);
                }

                filePath = Path.Combine(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath, newFileName);

                returnUrl = Path.Combine(tempUploadFiles + newFilePath, newFileName);

                var arr = Convert.FromBase64String(imageStr.Split(',')[1]);

                File.WriteAllBytes(filePath, arr);


                //剪切图片处理，前端已经做了处理 先注释以后有需求在看
                //using (MemoryStream ms = new MemoryStream(arr))
                //{
                //    Image img = new Bitmap(ms);
                //    int h = img.Height;
                //    int w = img.Width;
                //    Cut_Helper rect = new Cut_Helper();
                //    //判断图片大小来剪切图片比例，中心点开始截取
                //    if (w / 320 >= 6)
                //    {
                //        rect.Width = 320 * 6;
                //    }
                //    else if (w / 320 >= 5 && w / 320 < 6)
                //    {
                //        rect.Width = 320 * 5;
                //    }
                //    else if (w / 320 >= 4 && w / 320 < 5)
                //    {
                //        rect.Width = 320 * 4;
                //    }
                //    else if (w / 320 >= 3 && w / 320 < 4)
                //    {
                //        rect.Width = 320 * 3;
                //    }
                //    else if (w / 320 >= 2 && w / 320 < 3)
                //    {
                //        rect.Width = 320 * 2;
                //    }
                //    else
                //    {
                //        rect.Width = 320;
                //    }

                //    rect.Height = (int)(double.Parse(rect.Width.ToString()) * 0.5625);
                //    rect.X = (w - rect.Width) / 2;
                //    rect.Y = (h - rect.Height) / 2;

                //    File.WriteAllBytes(filePath, BitmapByte(CutImage(ms, rect)));
                //}
                #endregion

                PictureInfo pic = new PictureInfo();
                pic.Name = newFileName;
                pic.ImgFile = returnUrl;
                pic.picCreateTime = DateTime.Now;
                pic.picHot = 0;
                pic.picDescribe = newFileName;//照片描述，默认为照片的名称

                pic.uAccount = uAccount;
                pic.PicClsID = picClsID;

                db.PictureInfo.Add(pic);

                //更新所上传相册的照片量
                var picAll = await db.PictureInfo.Where(a => a.uAccount == uAccount&&a.PicClsID== picClsID).ToListAsync();

                PictureClass picCls = db.PictureClass.Find(picClsID);
                picCls.picClsPicCnt = picAll.Count + 1;

                await db.SaveChangesAsync();

                fg.Flag = true;
                list.Add(fg);
                return list;
            }
            catch (Exception ex)
            {
                fg.Flag = false;
                list.Add(fg);
                return list;
            }
        }

        [Route(Version_Helper.versionNumber + "/picture_/get")]
        public IEnumerable<PictureHelper> GetPictureInfo(string uAccount, string picClsID)
        {

            int ClsID = int.Parse(picClsID);

            if (uAccount == null)
            {
                return null;
            }

            List<PictureHelper> PictureInfo = (from p in db.PictureInfo
                                               join c in db.PictureClass
                                               on p.PicClsID equals c.ID
                                               where p.PicClsID == ClsID
                                               select new PictureHelper
                                               {
                                                   ID = p.ID,
                                                   Name = p.Name,
                                                   ImgFile = p.ImgFile,
                                                   picClsTitle = c.picClsTitle,
                                                   picDescribe = p.picDescribe,
                                                   picCreateTime = (DateTime)p.picCreateTime
                                               }).OrderByDescending(p => p.picCreateTime).ToList();

            return PictureInfo;
        }

        [HttpPost]
        [Route(Version_Helper.versionNumber + "/picture_/cls-delete")]
        public async Task<IEnumerable<FlagHelper>> PostPictureClsByDeleteCls(string uAccount, int ID)
        {

            try
            {

                PictureClass picCls = await db.PictureClass.Where(a => a.ID == ID).FirstOrDefaultAsync();

                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
                if (picCls == null)
                {
                    fg.Flag = false;
                    list.Add(fg);
                    return list;
                }
                if (picCls.uAccount != uAccount)
                {
                    fg.Flag = false;
                    list.Add(fg);
                    return list;
                }

                var oldPictures = await db.PictureInfo.Where(p => p.PicClsID == ID).ToListAsync();

                var defaultPicCls = await db.PictureClass.Where(p => p.picClsTitle == "默认相册" && p.uAccount == uAccount).ToListAsync();
                int defaultPicClsID = defaultPicCls.FirstOrDefault().ID;

                foreach (var item in oldPictures)
                {
                    item.PicClsID = defaultPicClsID;
                }

                db.PictureClass.Remove(picCls);

                await db.SaveChangesAsync();

                fg.Flag = true;
                list.Add(fg);
                return list;
            }
            catch (Exception)
            {
                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
                fg.Flag = false;
                list.Add(fg);
                return list;
            }

        }

        [HttpPost]
        [Route(Version_Helper.versionNumber + "/picture_/cls-update")]
        public async Task<IEnumerable<FlagHelper>> PostPictureClsByEditCls(string uAccount, string picClsTitle,
         int picClsAuthority, string picClsDescribe, int ID)
        {
            List<FlagHelper> list = new List<FlagHelper>();
            FlagHelper fg = new FlagHelper();

            if (uAccount == null)
            {
                fg.Flag = false;
                list.Add(fg);
                return list;
            }

            try
            {
                PictureClass picCls = await db.PictureClass.Where(p => p.ID == ID).FirstOrDefaultAsync();

                picCls.picClsTitle = picClsTitle;
                picCls.picClsAuthority = picClsAuthority;
                picCls.picClsDescribe = picClsDescribe;
                picCls.picClsCreateTime = DateTime.Now;

                HttpFileCollection files = HttpContext.Current.Request.Files;
                int mm = files.Count;
                if (mm != 0)
                {
                    #region             
                    var tempUploadFiles = "/Upload/PictureClsCoverFiles/";
                    var newFileName = "";
                    string filePath = "";
                    string extname = "";
                    string returnurl = "";

                    var filename = files.Get("imgFile").FileName;

                    FileInfo file = new FileInfo(filename);

                    string fileTypes = "gif,jpg,jpeg,png,bmp";

                    if (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) == -1)
                    {
                        throw new ApplicationException("不支持上传文件类型");
                    }

                    extname = filename.Substring(filename.LastIndexOf('.'), (filename.Length - filename.LastIndexOf('.')));

                    newFileName = Guid.NewGuid().ToString().Substring(0, 6) + uAccount + extname;


                    string newFilePath = DateTime.Now.ToString("yyyy-MM-dd") + "/";

                    if (!Directory.Exists(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath))
                    {
                        Directory.CreateDirectory(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath);
                    }
                    filePath = Path.Combine(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath, newFileName);

                    returnurl = Path.Combine(tempUploadFiles + newFilePath, newFileName);

                    Image img = new Bitmap(files.Get("imgFile").InputStream);
                    int h = img.Height;
                    int w = img.Width;
                    Cut_Helper rect = new Cut_Helper();
                    //判断图片大小来剪切图片比例，中心点开始截取
                    if (w / 165 >= 4 && w / 165 <= 6)
                    {
                        rect.Width = 165 * 2;
                    }
                    else if (w / 165 >= 6)
                    {
                        rect.Width = 165 * 4;
                    }
                    else
                    {
                        rect.Width = 165;
                    }

                    rect.Height = (int)(double.Parse(rect.Width.ToString()) / 0.75);
                    rect.X = (w - rect.Width) / 2;
                    rect.Y = (h - rect.Height) / 2;

                    byte[] ms = BitmapByte(CutImage(files.Get("imgFile").InputStream, rect));

                    if (ms.Length > 1048576 * 5)
                    {
                        throw new ApplicationException("文件太大");
                    }
                    else
                    {
                        File.WriteAllBytes(filePath, ms);
                    }

                    #endregion
                    picCls.CoverFile = returnurl;
                }

                await db.SaveChangesAsync();

                fg.Flag = true;
                list.Add(fg);
                return list;
            }
            catch (Exception)
            {
                fg.Flag = false;
                list.Add(fg);
                return list;
            }
        }

        [HttpPost]
        [Route(Version_Helper.versionNumber + "/picture_/cls-add")]
        public async Task<IEnumerable<FlagHelper>> PostPictureClsByAddCls(string uAccount, string picClsTitle,
            int picClsAuthority, string picClsDescribe)
        {
            List<FlagHelper> list = new List<FlagHelper>();
            FlagHelper fg = new FlagHelper();

            if (uAccount == null)
            {
                fg.Flag = false;
                list.Add(fg);
                return list;
            }

            try
            {
                #region             
                HttpFileCollection files = HttpContext.Current.Request.Files;
                int mm = files.Count;
                var tempUploadFiles = "/Upload/PictureClsCoverFiles/";
                var newFileName = "";
                string filePath = "";
                string extname = "";
                string returnurl = "";

                var filename = files.Get("imgFile").FileName;

                FileInfo file = new FileInfo(filename);

                string fileTypes = "gif,jpg,jpeg,png,bmp";

                if (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) == -1)
                {
                    throw new ApplicationException("不支持上传文件类型");
                }

                extname = filename.Substring(filename.LastIndexOf('.'), (filename.Length - filename.LastIndexOf('.')));

                newFileName = Guid.NewGuid().ToString().Substring(0, 6) + uAccount + extname;


                string newFilePath = DateTime.Now.ToString("yyyy-MM-dd") + "/";

                if (!Directory.Exists(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath))
                {
                    Directory.CreateDirectory(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath);
                }
                filePath = Path.Combine(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath, newFileName);

                returnurl = Path.Combine(tempUploadFiles + newFilePath, newFileName);

                Image img = new Bitmap(files.Get("imgFile").InputStream);
                int h = img.Height;
                int w = img.Width;
                Cut_Helper rect = new Cut_Helper();
                //判断图片大小来剪切图片比例，中心点开始截取
                if (w / 165 >= 4 && w / 165 <= 6)
                {
                    rect.Width = 165 * 2;
                }
                else if (w / 165 >= 6)
                {
                    rect.Width = 165 * 4;
                }
                else
                {
                    rect.Width = 165;
                }

                rect.Height = (int)(double.Parse(rect.Width.ToString()) / 0.75);
                rect.X = (w - rect.Width) / 2;
                rect.Y = (h - rect.Height) / 2;

                byte[] ms = BitmapByte(CutImage(files.Get("imgFile").InputStream, rect));

                if (ms.Length > 1048576 * 5)
                {
                    throw new ApplicationException("文件太大");
                }
                else
                {
                    File.WriteAllBytes(filePath, ms);
                }

                #endregion

                PictureClass picCls = new PictureClass();
                picCls.uAccount = uAccount;
                picCls.picClsAuthority = picClsAuthority;
                picCls.picClsTitle = picClsTitle;
                picCls.picClsDescribe = picClsDescribe;
                picCls.picClsCreateTime = DateTime.Now;
                picCls.CoverFile = returnurl;

                db.PictureClass.Add(picCls);
                await db.SaveChangesAsync();

                fg.Flag = true;
                list.Add(fg);
                return list;
            }
            catch (Exception)
            {
                fg.Flag = false;
                list.Add(fg);
                return list;
            }
        }
        private Bitmap CutImage(Stream stream, Cut_Helper cut)
        {
            Bitmap bmp = new Bitmap(stream);

            return bmp.Clone(new Rectangle(cut.X, cut.Y, cut.Width, cut.Height), PixelFormat.DontCare);
        }

        private static byte[] BitmapByte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }


        [Route(Version_Helper.versionNumber + "/picture_/cls-random")]
        public IEnumerable<PictureHelper> GetPictureInfoRandom(string uAccount)
        {

            List<PictureHelper> PictureInfo = (from i in db.PictureInfo
                                               where i.uAccount == uAccount
                                               select new PictureHelper
                                               {
                                                   ID = i.ID,
                                                   Name = i.Name,
                                                   ImgFile = i.ImgFile,
                                                   picCreateTime = (DateTime)i.picCreateTime,
                                                   picHot = i.picHot,
                                                   PicClsID = i.PicClsID,
                                                   picDescribe = i.picDescribe,
                                                   uAccount = i.uAccount

                                               }).ToList();

            List<PictureHelper> rndList = new List<PictureHelper>();

            if (PictureInfo.Count > 4)
            {
                int[] intArr = new int[4];

                Random rnd = new Random();
                while (rndList.Count < 4)
                {
                    int num = rnd.Next(1, PictureInfo.Count);
                    if (!rndList.Contains(PictureInfo[num]))
                    {
                        rndList.Add(PictureInfo[num]);
                    }
                }
                return rndList;
            }
            else
            {
                return PictureInfo;
            }

        }


        [Route(Version_Helper.versionNumber + "/picture_/cls")]
        public IEnumerable<PictureClsHelper> GetPictureCls(string uAccount)
        {
            var picCls = (from p in db.PictureClass
                          where p.uAccount == uAccount
                          select new PictureClsHelper
                          {
                              ID = p.ID,
                              picClsTitle = p.picClsTitle,
                              picClsDescribe = p.picClsDescribe,
                              CoverFile = p.CoverFile,
                              uAccount = p.uAccount,
                              picClsPicCnt = p.picClsPicCnt,
                              picClsCreateTime = p.picClsCreateTime,
                              picClsAuthority = p.picClsAuthority
                          }).OrderByDescending(c => c.picClsCreateTime).ToList();

            return picCls;

        }

        [Route(Version_Helper.versionNumber + "/picture_/random")]
        public IEnumerable<PictureHelper> GetPictureInfoRandom(string Account, int Times)
        {
            int count = 40;
            int page = Times;

            List<PictureHelper> PictureInfo = (from i in db.PictureInfo
                                               join c in db.PictureClass on i.PicClsID equals c.ID
                                               orderby i.picCreateTime descending
                                               where c.picClsAuthority == 1
                                               select new PictureHelper
                                               {
                                                   ID = i.ID,
                                                   Name = i.Name,
                                                   picClsTitle = c.picClsTitle,
                                                   ImgFile = i.ImgFile,
                                                   picCreateTime = (DateTime)i.picCreateTime,
                                                   picHot = i.picHot,
                                                   PicClsID = i.PicClsID,
                                                   picDescribe = i.picDescribe,
                                                   uAccount = i.uAccount,
                                                   Times = Times + 1,

                                               }).Skip((page - 1) * count).Take(count).ToList();


            List<PictureHelper> rndList = new List<PictureHelper>();
            if (PictureInfo.Count > count / 2)
            {
                int[] intArr = new int[count / 2];

                Random rnd = new Random();
                while (rndList.Count < count / 2)
                {
                    int num = rnd.Next(1, PictureInfo.Count);
                    if (!rndList.Contains(PictureInfo[num]))
                    {
                        rndList.Add(PictureInfo[num]);
                    }
                }
                return rndList;
            }
            else
            {
                return PictureInfo;
            }

        }

        [Route(Version_Helper.versionNumber + "/picture_/hot")]
        public IEnumerable<PictureHelper> GetPictureInfoHot(int page, int count)
        {

            List<PictureHelper> PictureInfo = (from i in db.PictureInfo
                                               join c in db.PictureClass on i.PicClsID equals c.ID
                                               orderby i.picCreateTime descending
                                               where c.picClsAuthority == 1
                                               select new PictureHelper
                                               {
                                                   ID = i.ID,
                                                   Name = i.Name,
                                                   picClsTitle = c.picClsTitle,
                                                   ImgFile = i.ImgFile,
                                                   picCreateTime = (DateTime)i.picCreateTime,
                                                   picHot = i.picHot,
                                                   PicClsID = i.PicClsID,
                                                   picDescribe = i.picDescribe,
                                                   uAccount = i.uAccount

                                               }).Skip((page - 1) * count).Take(count).ToList();
            return PictureInfo;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PictureInfoExists(int id)
        {
            return db.PictureInfo.Count(e => e.ID == id) > 0;
        }
    }
}