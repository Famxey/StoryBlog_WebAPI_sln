using StoryBlog_WebAPI.HelperCls;
using StoryBlog_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace StoryBlog_WebAPI.Controllers
{
    public class MyController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();



        [Route(Version_Helper.versionNumber + "/my/info")]
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> GetUserInfoByMy(string Account)
        {
            List<UserInfoHelper> userInfo = await (from a in db.UserInfo
                                                   where a.Account == Account
                                                   select new UserInfoHelper
                                                   {
                                                       Account = a.Account,
                                                       NickName = a.NickName,
                                                       Picture = a.Picture,
                                                       BGPicture = a.BGPicture,
                                                       Introduce = a.Introduce,
                                                   }).ToListAsync();


            if (userInfo == null)
            {
                return NotFound();
            }

            return Ok(userInfo);
        }

        [Route(Version_Helper.versionNumber + "/my/btnnum")]
        [ResponseType(typeof(MyBtnNumHelperCls))]
        public async Task<IHttpActionResult> GetUserInfoByMyBtnNum(string Account)
        {
            List<MyBtnNumHelperCls> btnNumList =new List<MyBtnNumHelperCls>();

            var articles = await db.ArticleInfo.Where(a => a.uAccount == Account).ToListAsync();


            var picCls= await db.PictureClass.Where(a => a.uAccount == Account).ToListAsync();
            var pictures = await db.PictureInfo.Where(p => p.uAccount == Account).ToListAsync();

            MyBtnNumHelperCls btnNum = new MyBtnNumHelperCls();
            btnNum.BlogsBtnNum = articles.Count;
            btnNum.PicturesBtnNum = picCls.Count.ToString()+"【"+ pictures.Count.ToString()+"】";

            btnNum.VideosBtnNum = 0;
            btnNum.TalksBtnNum = 0;
            btnNum.AttentionsBtnNum = 0;
            btnNum.EnshrinesBtnNum = 0;

            btnNumList.Add(btnNum);

            if (btnNumList == null)
            {
                return NotFound();
            }

            return Ok(btnNumList);
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

    public class MyBtnNumHelperCls
    {
        public int BlogsBtnNum { get; set; }
        public string PicturesBtnNum { get; set; }
        public int VideosBtnNum { get; set; }
        public int TalksBtnNum { get; set; }
        public int EnshrinesBtnNum { get; set; }
        public int AttentionsBtnNum { get; set; }

    }

}
