using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using StoryBlog_WebAPI.Models;

namespace StoryBlog_WebAPI.Controllers
{
    public class UserInfoesController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();

        // GET: api/UserInfoes
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


        // GET: api/UserInfoes/5
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


        [Route("api/UpdateUserInfo")]
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


        [Route("api/UpdateUserImage")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> UpdateUserImage(string Account, MultipartFormDataContent form)
        {

            if (Account==null)
            {
                return BadRequest();
            }
            if (form == null)
            {
                return BadRequest();
            }

            try
            {
                #region
                Stream stream = await form.ReadAsStreamAsync();

                string imageName = form.Headers.ContentDisposition.FileName;

                string filePath = "~/UploadPicture/HeadPicture/" + imageName;

                System.Drawing.Image ResourceImage = System.Drawing.Image.FromStream(stream);

                ResourceImage.Save(filePath);
                #endregion

                UserInfo userInfo = await db.UserInfo.FindAsync(Account);

                userInfo.Picture = filePath;

                await db.SaveChangesAsync();

                return Ok("yes");
            }
            catch (Exception)
            {
                return Ok("no");
            }
        }

        // POST: api/UserInfoes
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> PostUserInfo(UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserInfo.Add(userInfo);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserInfoExists(userInfo.Account))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = userInfo.Account }, userInfo);
        }



        // DELETE: api/UserInfoes/5
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> DeleteUserInfo(string id)
        {
            UserInfo userInfo = await db.UserInfo.FindAsync(id);
            if (userInfo == null)
            {
                return NotFound();
            }

            db.UserInfo.Remove(userInfo);
            await db.SaveChangesAsync();

            return Ok(userInfo);
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