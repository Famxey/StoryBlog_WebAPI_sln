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
    public class AdministrationsController : ApiController
    {
        private StoryBlog_DBEntities db = new StoryBlog_DBEntities();

        // GET: api/Administrations
        public IQueryable<Administration> GetAdministration()
        {
            return db.Administration;
        }

        // GET: api/Administrations/5
        [ResponseType(typeof(Administration))]
        public async Task<IHttpActionResult> GetAdministration(int id)
        {
            Administration administration = await db.Administration.FindAsync(id);
            if (administration == null)
            {
                return NotFound();
            }

            return Ok(administration);
        }

        // PUT: api/Administrations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdministration(int id, Administration administration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != administration.ID)
            {
                return BadRequest();
            }

            db.Entry(administration).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministrationExists(id))
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

        // POST: api/Administrations
        [ResponseType(typeof(Administration))]
        public async Task<IHttpActionResult> PostAdministration(Administration administration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Administration.Add(administration);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = administration.ID }, administration);
        }

        // DELETE: api/Administrations/5
        [ResponseType(typeof(Administration))]
        public async Task<IHttpActionResult> DeleteAdministration(int id)
        {
            Administration administration = await db.Administration.FindAsync(id);
            if (administration == null)
            {
                return NotFound();
            }

            db.Administration.Remove(administration);
            await db.SaveChangesAsync();

            return Ok(administration);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdministrationExists(int id)
        {
            return db.Administration.Count(e => e.ID == id) > 0;
        }
    }
}