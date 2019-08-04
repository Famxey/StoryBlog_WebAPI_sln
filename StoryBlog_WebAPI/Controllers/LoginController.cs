using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using StoryBlog_WebAPI.Models;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Diagnostics;
using StoryBlog_WebAPI.HelperCls;

namespace StoryBlog_WebAPI.Controllers
{
    public class LoginController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();

        [Route(Version_Helper.versionNumber + "/login/verification")]
        [ResponseType(typeof(UserInfoHelper))]
        public async Task<IHttpActionResult> PostLoginVerification(List<UserInfo> lui)
        {

            List<UserInfoHelper> list = await Verification(lui[0].Account, lui[0].PassWord);

            return Ok(list);
        }


        private async Task<List<UserInfoHelper>> Verification(string Account, string PassWord)
        {
            List<UserInfoHelper> userInfo = await (from a in db.UserInfo
                                                   where a.Account == Account && a.PassWord == PassWord
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
                                                       Flag = true
                                                   }).ToListAsync();

 

            Debug.WriteLine(userInfo);

            if (userInfo.Count == 0)
            {
                UserInfoHelper u = new UserInfoHelper();
                u.Flag = false;
                userInfo.Add(u);
                return userInfo;
            }
            else
            {
                return userInfo;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
