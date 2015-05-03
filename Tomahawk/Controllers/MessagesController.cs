using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tomahawk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace Tomahawk.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private TomahawkContext db;
        private UserManager<MyUser> manager;

        public MessagesController()
        {
            db = new TomahawkContext();
            manager = new UserManager<MyUser>(new UserStore<MyUser>(db));
        }

        // GET: Messages
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return View(db.Messages.ToList().Where(msg => msg.User.Id == currentUser.Id));
        }

        [HttpGet]
        public async Task<JsonResult> All()
        {
            // This is a clever way to avoid circular references with JSON serialization
            // source: http://blog.davebouwman.com/2011/12/08/handling-circular-references-asp-net-mvc-json-serialization/
            var data = await db.Messages.ToListAsync();
            var collection = data.Select(x => new
            {
                id = x.ID,
                text = x.Text,
                name = x.User.UserName
            });

            return Json(collection, JsonRequestBehavior.AllowGet);
        }

        // GET: Messages/Details/5
        public async Task<JsonResult> Details(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return Json(new Dictionary<string, bool> {
                    { "status", false }
                });
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return Json(new Dictionary<string, bool> {
                    { "status", false }
                });
            }
            if (message.User.Id != currentUser.Id)
            {
                return Json(new Dictionary<string, bool> {
                    { "status", false }
                });
            }

            var json = JsonConvert.SerializeObject(message, Formatting.Indented, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        // GET: Messages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([Bind(Include = "ID,Text")] Message message)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                message.User = currentUser;
                db.Messages.Add(message);
                await db.SaveChangesAsync();
                return Json(new Dictionary<string, bool> {
                    { "status", true }
                });
            }

            return Json(new Dictionary<string, bool> {
                { "status", false }
            });
        }

        // GET: Messages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Text")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            if (message.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            Message message = await db.Messages.FindAsync(id);

            if (message.User.Id != currentUser.Id)
            {
                return Json(new Dictionary<string, bool> {
                    { "status", false }
                });
            }

            db.Messages.Remove(message);
            await db.SaveChangesAsync();
            return Json(new Dictionary<string, bool> {
                { "status", true }
            });
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
