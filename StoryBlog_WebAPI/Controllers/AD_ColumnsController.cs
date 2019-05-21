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
    public class AD_ColumnsController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();

        // GET: api/AD_Columns
        public IQueryable<AD_Columns> GetAD_Columns()
        {
            return db.AD_Columns;
        }

        // GET: api/AD_Columns/5
        [ResponseType(typeof(AD_Columns))]
        public async Task<IHttpActionResult> GetAD_Columns(int id)
        {
            AD_Columns aD_Columns = await db.AD_Columns.FindAsync(id);
            if (aD_Columns == null)
            {
                return NotFound();
            }

            return Ok(aD_Columns);
        }

        // PUT: api/AD_Columns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAD_Columns(int id, AD_Columns aD_Columns)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aD_Columns.ID)
            {
                return BadRequest();
            }

            db.Entry(aD_Columns).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AD_ColumnsExists(id))
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

        // POST: api/AD_Columns
        [ResponseType(typeof(AD_Columns))]
        public async Task<IHttpActionResult> PostAD_Columns(AD_Columns aD_Columns)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AD_Columns.Add(aD_Columns);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = aD_Columns.ID }, aD_Columns);
        }

        // DELETE: api/AD_Columns/5
        [ResponseType(typeof(AD_Columns))]
        public async Task<IHttpActionResult> DeleteAD_Columns(int id)
        {
            AD_Columns aD_Columns = await db.AD_Columns.FindAsync(id);
            if (aD_Columns == null)
            {
                return NotFound();
            }

            db.AD_Columns.Remove(aD_Columns);
            await db.SaveChangesAsync();

            return Ok(aD_Columns);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AD_ColumnsExists(int id)
        {
            return db.AD_Columns.Count(e => e.ID == id) > 0;
        }
    }
}