using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using StoryBlog_WebAPI.HelperCls;
using StoryBlog_WebAPI.Models;

namespace StoryBlog_WebAPI.Controllers
{
    public class ArticleInfoesController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();


        [HttpPost]
        [Route(Version_Helper.versionNumber + "/article_/delete")]
        public async Task<IEnumerable<FlagHelper>> PostArticleInfoByDelete(string uAccount, string artNo)
        {

            try
            {

                ArticleInfo articleInfo = await db.ArticleInfo.Where(a => a.artNo == artNo).FirstOrDefaultAsync();

                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
                if (articleInfo == null)
                {
                    fg.Flag = false;
                    list.Add(fg);
                    return list;
                }
                if (articleInfo.uAccount != uAccount)
                {
                    fg.Flag = false;
                    list.Add(fg);
                    return list;
                }

                db.ArticleInfo.Remove(articleInfo);

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
        [Route(Version_Helper.versionNumber + "/article_/update")]
        public async Task<IEnumerable<FlagHelper>> PostArticleInfoByUpdate(ArticleInfo ai)
        {

            try
            {
                ArticleInfo art = await db.ArticleInfo.Where(a => a.artNo == ai.artNo).FirstOrDefaultAsync();
                art.Title = ai.Title;
                art.artContent = ai.artContent;
                art.artCreateTime = DateTime.Now;
                art.artClsID = ai.artClsID;
                art.uAccount = ai.uAccount;
                art.artAuthority = ai.artAuthority;
                art.artDigest = ai.artDigest.Trim();

                await db.SaveChangesAsync();

                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
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
        [Route(Version_Helper.versionNumber + "/article_/add")]
        public async Task<IEnumerable<FlagHelper>> PostArticleInfoByAdd(ArticleInfo art)
        {

            try
            {
                art.artNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                art.artCreateTime = DateTime.Now;
                art.artHot = 0;
                art.artComCnt = 0;

                db.ArticleInfo.Add(art);
                await db.SaveChangesAsync();

                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
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
        [Route(Version_Helper.versionNumber + "/article_/cls_add")]
        public async Task<IEnumerable<FlagHelper>> PostArticleInfoByClsAdd(string uAccount, string artClsTitle)
        {
            try
            {
                ArticleClass cls = new ArticleClass();
                cls.uAccount = uAccount;
                cls.artClsTitle = artClsTitle;
                db.ArticleClass.Add(cls);
                await db.SaveChangesAsync();
                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
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


        [Route(Version_Helper.versionNumber + "/article_/cls")]
        public IEnumerable<ArticleClsHelper> GetArticleInfoByCls(string uAccount)
        {
            var Cls = (from a in db.ArticleClass
                       where a.uAccount == uAccount
                       select new ArticleClsHelper
                       {
                           ID = a.ID,
                           artClsTitle = a.artClsTitle,
                           uAccount = a.uAccount
                       }).ToList();
            return Cls;

        }


        [Route(Version_Helper.versionNumber + "/article_/comment_add")]
        public async Task<IEnumerable<FlagHelper>> GetArticleInfoByCommentAdd(string uAccount, string artNo, string artCmContent)
        {
            try
            {

                var art =await db.ArticleInfo.Where(a => a.artNo == artNo).FirstOrDefaultAsync();

                int artID = art.ID;

                ArtComment artCM = new ArtComment();
                artCM.artCmCreateTime = DateTime.Now;
                artCM.artID = artID;
                artCM.artCmContent = artCmContent;
                if (uAccount != "" && uAccount != null && uAccount != "null")
                    artCM.uAccount = uAccount;
                else
                    artCM.uAccount = "cyan";

                db.ArtComment.Add(artCM);
                await db.SaveChangesAsync();

                //更新评论数量
                var acm = db.ArtComment.Where(c => c.artID == artCM.artID).ToList();
                int artcmcn = acm.Count;
                ArticleInfo ai =await db.ArticleInfo.FindAsync(artCM.artID);
                ai.artComCnt = artcmcn;
                await db.SaveChangesAsync();

                List<FlagHelper> list = new List<FlagHelper>();
                FlagHelper fg = new FlagHelper();
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

        [Route(Version_Helper.versionNumber + "/article_/comment")]
        public IEnumerable<ArticleCommentHelper> GetArticleInfoByComment(string artNo)
        {
            int ID = db.ArticleInfo.Where(u => u.artNo == artNo).FirstOrDefault().ID;

            var artCM = (from i in db.ArtComment
                         join u in db.UserInfo on i.uAccount equals u.Account
                         orderby i.artCmCreateTime descending
                         where i.artID == ID
                         select new ArticleCommentHelper
                         {
                             artCmContent = i.artCmContent,
                             NickName = u.NickName,
                             uAccount = u.Account,
                             artCmCreateTime = i.artCmCreateTime,
                             uPicture = u.Picture
                         }).ToList();

            return artCM;
        }

        [Route(Version_Helper.versionNumber + "/article_/page_user")]
        public IEnumerable<ArticleHelper> GetArticleInfoByPage(string artNo)
        {

            List<ArticleHelper> pageMiddle = (from a in db.ArticleInfo
                                              where a.artNo == artNo
                                              select new ArticleHelper
                                              {
                                                  artCreateTime = a.artCreateTime,
                                                  uAccount = a.uAccount
                                              }).ToList();

            DateTime dt = pageMiddle[0].artCreateTime;

            string Account = pageMiddle[0].uAccount;

            List<ArticleHelper> pageUp = (from a in db.ArticleInfo
                                          orderby a.artCreateTime descending
                                          where a.uAccount == Account && a.artCreateTime > dt
                                          select new ArticleHelper
                                          {
                                              artCreateTime = a.artCreateTime,
                                              artNo = a.artNo,
                                              pageUp = true,
                                              pageDwon = true
                                          }).OrderBy(t => t.artCreateTime).Take(1).ToList();

            List<ArticleHelper> pageDown = (from a in db.ArticleInfo
                                            orderby a.artCreateTime descending
                                            where a.uAccount == Account && a.artCreateTime < dt
                                            select new ArticleHelper
                                            {
                                                artCreateTime = a.artCreateTime,
                                                artNo = a.artNo,
                                                pageUp = true,
                                                pageDwon = true
                                            }).Take(1).ToList();

            if (pageUp.Count == 0)
            {
                pageDown[0].pageUp = false;
                return pageDown;
            }
            else if (pageDown.Count == 0)
            {
                pageUp[0].pageDwon = false;
                return pageUp;
            }
            else if (pageUp.Count == 0 && pageDown.Count == 0)
            {
                return null;
            }
            else
            {
                return pageUp.Union(pageDown);
            }


        }


        [Route(Version_Helper.versionNumber + "/article_/list_user")]
        public IEnumerable<ArticleHelper> GetArticleInfoByList(string artNo)
        {

            List<ArticleHelper> pageMiddle = (from a in db.ArticleInfo
                                              where a.artNo == artNo
                                              select new ArticleHelper
                                              {
                                                  artNo = a.artNo,
                                                  Title = a.Title,
                                                  artCreateTime = a.artCreateTime,
                                                  uAccount = a.uAccount
                                              }).ToList();

            DateTime dt = pageMiddle[0].artCreateTime;

            string Account = pageMiddle[0].uAccount;

            List<ArticleHelper> pageUp = (from a in db.ArticleInfo
                                          orderby a.artCreateTime descending
                                          where a.uAccount == Account && a.artCreateTime > dt
                                          select new ArticleHelper
                                          {
                                              artNo = a.artNo,
                                              Title = a.Title,
                                              artCreateTime = a.artCreateTime
                                          }).OrderBy(t => t.artCreateTime).Take(4).ToList();

            List<ArticleHelper> pageDown = (from a in db.ArticleInfo
                                            orderby a.artCreateTime descending
                                            where a.uAccount == Account && a.artCreateTime < dt
                                            select new ArticleHelper
                                            {
                                                artNo = a.artNo,
                                                Title = a.Title,
                                                artCreateTime = a.artCreateTime
                                            }).Take(4).ToList();


            switch (pageUp.Count)
            {
                case 0:
                    return pageMiddle.Union(pageDown.Take(4)).OrderByDescending(t => t.artCreateTime).ToList();
                case 1:
                    return pageMiddle.Union(pageDown.Take(3)).Union(pageUp.Take(1)).OrderByDescending(t => t.artCreateTime).ToList();
                default:
                    return pageMiddle.Union(pageDown.Take(2)).Union(pageUp.Take(2)).OrderByDescending(t => t.artCreateTime).ToList();
            }

        }


        [Route(Version_Helper.versionNumber + "/article_/info")]
        public IEnumerable<ArticleHelper> GetArticleInfoByInfo(string artNo)
        {

            List<ArticleHelper> ArticleInfo = (from i in db.ArticleInfo
                                               join u in db.UserInfo
                                               on i.uAccount equals u.Account
                                               where i.artNo == artNo
                                               select new ArticleHelper
                                               {
                                                   ID = i.ID,
                                                   Title = i.Title,
                                                   artNo = i.artNo,
                                                   artCreateTime = i.artCreateTime,
                                                   NickName = u.NickName,
                                                   uAccount = i.uAccount,
                                                   artContent = i.artContent,
                                                   artHot = i.artHot,
                                                   artComCnt = i.artComCnt,
                                                   artDigest = i.artDigest,
                                                   artAuthority = i.artAuthority,
                                                   artClsID = i.artClsID
                                               }).ToList();

            return ArticleInfo;

        }


        [Route(Version_Helper.versionNumber + "/article_/user")]
        public IEnumerable<ArticleHelper> GetArticleInfoByUser(string Account, int Times)
        {
            int count = 20;
            int page = Times;

            List<ArticleHelper> ArticleInfo = (from a in db.ArticleInfo
                                               orderby a.artCreateTime descending
                                               where a.uAccount == Account
                                               select new ArticleHelper
                                               {
                                                   artNo = a.artNo,
                                                   Title = a.Title,
                                                   artHot = a.artHot,
                                                   artComCnt = a.artComCnt,
                                                   artCreateTime = a.artCreateTime,
                                                   Times = Times + 1,
                                               }).Skip((page - 1) * count).Take(count).ToList();

            return ArticleInfo;

        }


        [Route(Version_Helper.versionNumber + "/article_/random")]
        public IEnumerable<ArticleHelper> GetArticleInfoRandom(string Account, int Times)
        {
            int count = 40;
            int page = Times;

            List<ArticleHelper> ArticleInfo = (from a in db.ArticleInfo
                                               orderby a.artHot descending
                                               where a.artAuthority == 1
                                               select new ArticleHelper
                                               {
                                                   artNo = a.artNo,
                                                   artDigest = a.artDigest,
                                                   Title = a.Title,
                                                   artHot = a.artHot,
                                                   Times = Times + 1,
                                               }).Skip((page - 1) * count).Take(count).ToList();

            List<ArticleHelper> rndList = new List<ArticleHelper>();
            if (ArticleInfo.Count > count / 2)
            {
                int[] intArr = new int[count / 2];

                Random rnd = new Random();
                while (rndList.Count < count / 2)
                {
                    int num = rnd.Next(1, ArticleInfo.Count);
                    if (!rndList.Contains(ArticleInfo[num]))
                    {
                        rndList.Add(ArticleInfo[num]);
                    }
                }
                return rndList;
            }
            else
            {
                return ArticleInfo;
            }

        }


        [Route(Version_Helper.versionNumber + "/article_/hot")]
        public IEnumerable<ArticleHelper> GetArticleInfoHot(int page, int count)
        {
            List<ArticleHelper> ArticleInfo = (from a in db.ArticleInfo
                                               orderby a.artHot descending
                                               where a.artAuthority == 1
                                               select new ArticleHelper
                                               {
                                                   artNo = a.artNo,
                                                   artDigest = a.artDigest,
                                                   Title = a.Title,
                                                   artHot = a.artHot
                                               }).Skip((page - 1) * count).Take(count).ToList();
            return ArticleInfo;
        }

        [Route(Version_Helper.versionNumber + "/article_/get")]
        public IQueryable<ArticleInfo> GetArticleInfo()
        {
            return db.ArticleInfo;
        }

        [Route(Version_Helper.versionNumber + "/article_/get")]
        [ResponseType(typeof(ArticleInfo))]
        public async Task<IHttpActionResult> GetArticleInfo(int id)
        {
            ArticleInfo articleInfo = await db.ArticleInfo.FindAsync(id);
            if (articleInfo == null)
            {
                return NotFound();
            }

            return Ok(articleInfo);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ArticleInfoExists(int id)
        {
            return db.ArticleInfo.Count(e => e.ID == id) > 0;
        }
    }
}