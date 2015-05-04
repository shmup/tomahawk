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
            var messages = await db.Messages.OrderByDescending(m => m.ID).ToListAsync();
            var result = messages.Select(x => new
            {
                id = x.ID,
                text = x.Text,
                name = x.User.UserName
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Messages/Details/5
        public async Task<JsonResult> Details(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return Json(new {
                    success = false,
                    message = "Missing the ID"
                }, JsonRequestBehavior.AllowGet);
            }
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return Json(new {
                    success = false,
                    message = "Message does not exist with that ID"
                }, JsonRequestBehavior.AllowGet);
            }

            var replies = message.Replies.Select(r => new
            {
                id = r.ID,
                name = r.User.UserName,
                text = r.Text,
            });

            var result = new
                {
                    success = true,
                    message = new
                    {
                        id = message.ID,
                        text = message.Text,
                        name = message.User.UserName,
                    },
                    replies = replies
                };

            return Json(result, JsonRequestBehavior.AllowGet);
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

                return Json(new
                {
                    success = true,
                    id = message.ID,
                    text = message.Text,
                    name = message.User.UserName
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = false
            });
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
                return Json(new
                {
                    success = false,
                    message = "You are not authorized"
                }, JsonRequestBehavior.AllowGet);
            }

            db.Messages.Remove(message);
            await db.SaveChangesAsync();

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: Replies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ReplyCreate(int? Parent_ID, [Bind(Include = "ID,Text")] Reply reply)
        {
            try
            {
                var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
                Message message = await db.Messages.FindAsync(Parent_ID);
                reply.Parent = message;
                reply.User = currentUser;
                db.Replies.Add(reply);
                await db.SaveChangesAsync();
                return Json(new
                {
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    success = false
                }, JsonRequestBehavior.AllowGet);
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
