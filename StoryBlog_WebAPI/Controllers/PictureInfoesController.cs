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
    public class PictureInfoesController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();

        [Route(Version_Helper.versionNumber + "/picture_/random")]
        public IEnumerable<PictureHelper> GetArticleInfoHot(string Account, int Times)
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
                                                   picClsTitle=c.picClsTitle,
                                                   ImgFile = i.ImgFile,
                                                   picCreateTime = (DateTime)i.picCreateTime,
                                                   picHot=i.picHot,
                                                   PicClsID=i.PicClsID,
                                                   picDescribe = i.picDescribe,
                                                   uAccount=i.uAccount

                                               }).Skip((page - 1) * count).Take(count).ToList();
            return PictureInfo;
        }

        // GET: api/PictureInfoes
        public IQueryable<PictureInfo> GetPictureInfo()
        {
            return db.PictureInfo;
        }

        // GET: api/PictureInfoes/5
        [ResponseType(typeof(PictureInfo))]
        public async Task<IHttpActionResult> GetPictureInfo(int id)
        {
            PictureInfo pictureInfo = await db.PictureInfo.FindAsync(id);
            if (pictureInfo == null)
            {
                return NotFound();
            }

            return Ok(pictureInfo);
        }

        // PUT: api/PictureInfoes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPictureInfo(int id, PictureInfo pictureInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pictureInfo.ID)
            {
                return BadRequest();
            }

            db.Entry(pictureInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PictureInfoExists(id))
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

        // POST: api/PictureInfoes
        [ResponseType(typeof(PictureInfo))]
        public async Task<IHttpActionResult> PostPictureInfo(PictureInfo pictureInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PictureInfo.Add(pictureInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = pictureInfo.ID }, pictureInfo);
        }

        // DELETE: api/PictureInfoes/5
        [ResponseType(typeof(PictureInfo))]
        public async Task<IHttpActionResult> DeletePictureInfo(int id)
        {
            PictureInfo pictureInfo = await db.PictureInfo.FindAsync(id);
            if (pictureInfo == null)
            {
                return NotFound();
            }

            db.PictureInfo.Remove(pictureInfo);
            await db.SaveChangesAsync();

            return Ok(pictureInfo);
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