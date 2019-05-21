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
using StoryBlog_WebAPI.Models;

namespace StoryBlog_WebAPI.Controllers
{
    public class ArticleInfoesController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();

        [Route("api/GetArticleInfoHot")]
        public IEnumerable<ArticleHelper> GetArticleInfoHot(int page,int count)
        {
            List<ArticleHelper> ArticleInfo = (from a in db.ArticleInfo
                                               orderby a.artHot descending
                                               where a.artAuthority == 1
                                               select new ArticleHelper
                                               {
                                                   artNo = a.artNo,
                                                   artDigest = a.artDigest,
                                                   Title = a.Title,
                                                   artHot=a.artHot
                                               }).Skip((page-1)*count).Take(count).ToList();
            return ArticleInfo;
        }

        // GET: api/ArticleInfoes
        public IQueryable<ArticleInfo> GetArticleInfo()
        {
            return db.ArticleInfo;
        }

        // GET: api/ArticleInfoes/5
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

        // PUT: api/ArticleInfoes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutArticleInfo(int id, ArticleInfo articleInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != articleInfo.ID)
            {
                return BadRequest();
            }

            db.Entry(articleInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ArticleInfoes
        [ResponseType(typeof(ArticleInfo))]
        public async Task<IHttpActionResult> PostArticleInfo(ArticleInfo articleInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ArticleInfo.Add(articleInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = articleInfo.ID }, articleInfo);
        }

        // DELETE: api/ArticleInfoes/5
        [ResponseType(typeof(ArticleInfo))]
        public async Task<IHttpActionResult> DeleteArticleInfo(int id)
        {
            ArticleInfo articleInfo = await db.ArticleInfo.FindAsync(id);
            if (articleInfo == null)
            {
                return NotFound();
            }

            db.ArticleInfo.Remove(articleInfo);
            await db.SaveChangesAsync();

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