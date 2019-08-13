using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net.Http;

using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;

using StoryBlog_WebAPI.Models;
using StoryBlog_WebAPI.HelperCls;
using System.Drawing;
using System.Drawing.Imaging;

namespace StoryBlog_WebAPI.Controllers
{
    public class UserInfoesController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();

        [Route(Version_Helper.versionNumber + "/user_/get")]
        public IEnumerable<UserInfoHelper> GetUserInfo()
        {
            List<UserInfoHelper> userInfo = (from a in db.UserInfo
                                             select new UserInfoHelper
                                             {
                                                 Account = a.Account,
                                                 ID = a.ID,
                                                 NickName = a.NickName,
                                                 PassWord = a.PassWord,
                                                 Picture = a.Picture,
                                                 Phone = a.Phone,
                                                 Gender = a.Gender,
                                                 Age = a.Age,
                                                 Birthday = a.Birthday,
                                                 CreateTime = a.CreateTime,
                                                 LoginTime = a.LoginTime,
                                                 Describe = a.Describe,
                                                 Introduce = a.Introduce,
                                             }).ToList();
            return userInfo;
        }


        [Route(Version_Helper.versionNumber + "/user_/get")]
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> GetUserInfo(int id)
        {
            List<UserInfoHelper> userInfo = await (from a in db.UserInfo
                                                   where a.ID == id
                                                   select new UserInfoHelper
                                                   {
                                                       Account = a.Account,
                                                       ID = a.ID,
                                                       NickName = a.NickName,
                                                       PassWord = a.PassWord,
                                                       Picture = a.Picture,
                                                       Phone = a.Phone,
                                                       Gender = a.Gender,
                                                       Age = a.Age,
                                                       Birthday = a.Birthday,
                                                       CreateTime = a.CreateTime,
                                                       LoginTime = a.LoginTime,
                                                       Describe = a.Describe,
                                                       Introduce = a.Introduce,
                                                   }).ToListAsync();


            if (userInfo == null)
            {
                return NotFound();
            }

            return Ok(userInfo);
        }


        [Route(Version_Helper.versionNumber + "/user_/update")]
        [ResponseType(typeof(FlagHelper))]
        public async Task<IHttpActionResult> UpdateUserInfo(string Option, string Account, List<UserInfo> lui)
        {

            List<FlagHelper> list_f = new List<FlagHelper>();
            FlagHelper f = new FlagHelper();

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (Account != lui[0].Account)
                {
                    f.Flag = false;
                    list_f.Add(f);
                    return Ok(list_f);
                }

                UserInfo userInfo = await db.UserInfo.FindAsync(Account);

                if (userInfo == null)
                {
                    return NotFound();
                }

                switch (Option)
                {
                    case "NickName":
                        userInfo.NickName = lui[0].NickName;
                        break;
                    case "Phone":
                        userInfo.Phone = lui[0].Phone;
                        break;
                    case "Introduce":
                        userInfo.Introduce = lui[0].Introduce;
                        break;
                    case "Describe":
                        userInfo.Describe = lui[0].Describe;
                        break;
                    case "Gender":
                        userInfo.Gender = lui[0].Gender;
                        break;
                    case "Birthday":
                        userInfo.Birthday = lui[0].Birthday;
                        break;
                    case "PassWord":
                        userInfo.PassWord = lui[0].PassWord;
                        break;
                }

                await db.SaveChangesAsync();

                f.Flag = true;
                list_f.Add(f);
                return Ok(list_f);

            }
            catch (Exception)
            {
                f.Flag = false;
                list_f.Add(f);
                return Ok(list_f);
            }

        }


        [Route(Version_Helper.versionNumber + "/user_/update-croppedimgage")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> UpdateUserCroppedImage(string Account)
        {
            if (Account == null)
            {
                return BadRequest();
            }
         
            try
            {
                #region             
                var content = Request.Content;
                var tempUploadFiles = "/Upload/HeadPictureFiles/";
                var newFileName = "";
                string filePath = "";
                string extname = "";
                string authAccount = "";
                string returnurl = "";
                var sp = new MultipartMemoryStreamProvider();
                Task.Run(async () => await Request.Content.ReadAsMultipartAsync(sp)).Wait();

                foreach (var item in sp.Contents)
                {

                    if (item.Headers.ContentDisposition.FileName != null)
                    {
                        var filename = item.Headers.ContentDisposition.FileName.Replace("\"", "");

                        FileInfo file = new FileInfo(filename);

                        string fileTypes = "gif,jpg,jpeg,png,bmp";

                        if (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) == -1)
                        {
                            throw new ApplicationException("不支持上传文件类型");
                        }

                        //string[] strArray = filename.Split('.');
                        //authAccount = strArray[0];
                        //if (authAccount != Account)
                        //{
                        //    throw new ApplicationException("图片数据来源不明，不允许操作！");
                        //}

                        extname = filename.Substring(filename.LastIndexOf('.'), (filename.Length - filename.LastIndexOf('.')));

                        newFileName = Guid.NewGuid().ToString().Substring(0, 6) + Account + extname;


                        string newFilePath = DateTime.Now.ToString("yyyy-MM-dd") + "/";

                        if (!Directory.Exists(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath))
                        {
                            Directory.CreateDirectory(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath);
                        }
                        filePath = Path.Combine(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath, newFileName);

                        returnurl = Path.Combine(tempUploadFiles + newFilePath, newFileName);

                        //Stream ms = System.Web.HttpContext.Current.Request.InputStream;

                        var ms = item.ReadAsStreamAsync().Result;

                        using (var br = new BinaryReader(ms))
                        {
                            if (ms.Length > 1048576 * 5)
                            {
                                throw new ApplicationException("文件太大");
                            }
                            var data = br.ReadBytes((int)ms.Length);
                            File.WriteAllBytes(filePath, data);
                        }
                    }
                }

                #endregion

                UserInfo userInfo = await db.UserInfo.FindAsync(Account);

                userInfo.Picture = returnurl;

                await db.SaveChangesAsync();

                return Ok(returnurl);
            }
            catch (Exception)
            {
                return Ok("no");
            }
        }


        [Route(Version_Helper.versionNumber + "/user_/update-bgpicture")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> UpdateUserBGPicture(string Account)
        {
            if (Account == null)
            {
                return BadRequest();
            }

            try
            {
                #region             
                var content = Request.Content;
                var tempUploadFiles = "/Upload/BGPictureFiles/";
                var newFileName = "";
                string filePath = "";
                string extname = "";
                string authAccount = "";
                string returnurl = "";
                var sp = new MultipartMemoryStreamProvider();
                Task.Run(async () => await Request.Content.ReadAsMultipartAsync(sp)).Wait();

                foreach (var item in sp.Contents)
                {

                    if (item.Headers.ContentDisposition.FileName != null)
                    {
                        var filename = item.Headers.ContentDisposition.FileName.Replace("\"", "");

                        FileInfo file = new FileInfo(filename);

                        string fileTypes = "gif,jpg,jpeg,png,bmp";

                        if (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) == -1)
                        {
                            throw new ApplicationException("不支持上传文件类型");
                        }

                        extname = filename.Substring(filename.LastIndexOf('.'), (filename.Length - filename.LastIndexOf('.')));

                        newFileName = Guid.NewGuid().ToString().Substring(0, 6) + Account + extname;


                        string newFilePath = DateTime.Now.ToString("yyyy-MM-dd") + "/";

                        if (!Directory.Exists(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath))
                        {
                            Directory.CreateDirectory(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath);
                        }
                        filePath = Path.Combine(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath, newFileName);

                        returnurl = Path.Combine(tempUploadFiles + newFilePath, newFileName);

                        var ms = item.ReadAsStreamAsync().Result;

                        using (var br = new BinaryReader(ms))
                        {
                            if (ms.Length > 1048576 * 5)
                            {
                                throw new ApplicationException("文件太大");
                            }
                            var data = br.ReadBytes((int)ms.Length);
                            File.WriteAllBytes(filePath, data);
                        }
                    }
                }

                #endregion

                UserInfo userInfo = await db.UserInfo.FindAsync(Account);

                userInfo.BGPicture = returnurl;

                await db.SaveChangesAsync();

                return Ok(returnurl);
            }
            catch (Exception)
            {
                return Ok("no");
            }
        }


        [Route(Version_Helper.versionNumber + "/user_/update-imgage")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> UpdateUserImage(string Account, int X, int Y, int Width, int Height)
        {
            if (Account == null)
            {
                return BadRequest();
            }
            try
            {
                Cut_Helper cut = new Cut_Helper();
                cut.X = X;
                cut.Y = Y;
                cut.Width = Width;
                cut.Height = Height;

                #region             
                var content = Request.Content;
                var tempUploadFiles = "/Upload/HeadPictureFiles/";
                var newFileName = "";
                string filePath = "";
                string extname = "";
                string authAccount = "";
                string returnurl = "";
                var sp = new MultipartMemoryStreamProvider();
                Task.Run(async () => await Request.Content.ReadAsMultipartAsync(sp)).Wait();

                foreach (var item in sp.Contents)
                {

                    if (item.Headers.ContentDisposition.FileName != null)
                    {
                        var filename = item.Headers.ContentDisposition.FileName.Replace("\"", "");

                        FileInfo file = new FileInfo(filename);

                        string fileTypes = "gif,jpg,jpeg,png,bmp";

                        if (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) == -1)
                        {
                            throw new ApplicationException("不支持上传文件类型");
                        }

                        //string[] strArray = filename.Split('.');
                        //authAccount = strArray[0];
                        //if (authAccount != Account)
                        //{
                        //    throw new ApplicationException("图片数据来源不明，不允许操作！");
                        //}

                        extname = filename.Substring(filename.LastIndexOf('.'), (filename.Length - filename.LastIndexOf('.')));

                        newFileName = Guid.NewGuid().ToString().Substring(0, 6) + Account + extname;


                        string newFilePath = DateTime.Now.ToString("yyyy-MM-dd") + "/";

                        if (!Directory.Exists(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath))
                        {
                            Directory.CreateDirectory(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath);
                        }
                        filePath = Path.Combine(HostingEnvironment.MapPath("/") + tempUploadFiles + newFilePath, newFileName);

                        returnurl = Path.Combine(tempUploadFiles + newFilePath, newFileName);

                        var ms = item.ReadAsStreamAsync().Result;

                        Bitmap bmp = CutImage(ms, cut);

                        bmp.Save(filePath);

                        //using (var br = new BinaryReader(ms))
                        //{
                        //    if (ms.Length > 1048576 * 5)
                        //    {
                        //        throw new ApplicationException("文件太大");
                        //    }
                        //    var data = br.ReadBytes((int)ms.Length);
                        //    File.WriteAllBytes(filePath, data);
                        //}
                    }
                }

                #endregion

                UserInfo userInfo = await db.UserInfo.FindAsync(Account);

                userInfo.Picture = returnurl;

                await db.SaveChangesAsync();

                return Ok(returnurl);
            }
            catch (Exception)
            {
                return Ok("no");
            }
        }


        private Bitmap CutImage(Stream stream, Cut_Helper cut)
        {
            Bitmap bmp = new Bitmap(stream);

            return bmp.Clone(new Rectangle(cut.X, cut.Y, cut.Width, cut.Height), PixelFormat.DontCare);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private bool UserInfoExists(string id)
        {
            return db.UserInfo.Count(e => e.Account == id) > 0;
        }
    }
}