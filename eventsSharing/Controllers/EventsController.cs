using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eventsSharing;

namespace eventsSharing.Controllers
{
    public class EventsController : Controller
    {
        private eventDBEntities db = new eventDBEntities();

        // GET: Events
        public ActionResult Index()
        {
            var userId = HttpContext.Session["userId"];
            var role = HttpContext.Session["role"];

            if (role == null || userId == null )
            {
                return RedirectToAction("Login", "Home");
            }
            bool isManager = role.ToString().IndexOf("MANAGER") != -1;
            bool isAttendee = role.ToString().IndexOf("ATTENDEE") != -1;
            if ( (!isManager && !isAttendee))
            {
                return RedirectToAction("Login", "Home");
            }

            if (isAttendee)
            {
                return RedirectToAction("List");
            }
            var parsedUserId = int.Parse(userId.ToString());
            var events = db.Events.Where(a => a.ownerId == parsedUserId).ToList();
            
            var registrations = db.Registrations.ToList();
            foreach (var item in events)
            {
                item.registrantsCount = registrations.FindAll(e => e.eventId == item.id).Count;
            }
            return View(events);
        }

        // GET: Events/List
        public ActionResult List()
        {
            var userId = HttpContext.Session["userId"];
            var role = HttpContext.Session["role"];
            if (role == null || userId == null || role.ToString().IndexOf("ATTENDEE") == -1)
            {
                return RedirectToAction("Login", "Home");
            }

            var parsedUserId = int.Parse(userId.ToString());
            var registrations = db.Registrations.Where(a => a.userId == parsedUserId).ToList();
            var events = db.Events.Include(a => a.User).ToList();

            foreach (var item in events)
            {
                if(registrations.Find(e=>e.eventId == item.id) != null)
                {
                    item.isSelected = true;
                }
            }
            return View(events);
        }

        // GET: Events/Apply/5
        public ActionResult Apply(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        // POST: Events/Apply/5
        [HttpPost, ActionName("Apply")]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyConfirmed(int id)
        {
            var userId = HttpContext.Session["userId"];
            Events events = db.Events.Find(id);
            Registrations registrations = new Registrations();
            registrations.userId = int.Parse(userId.ToString());
            registrations.eventId = id;
            registrations.date = DateTime.UtcNow.Date;

            db.Registrations.Add(registrations);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,details,location,date")] Events events)
        {
            var userId = int.Parse(HttpContext.Session["userId"].ToString());
            events.ownerId = userId;
            if (ModelState.IsValid)
            {
                db.Events.Add(events);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(events);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            ViewBag.ownerId = new SelectList(db.User, "id", "name", events.ownerId);
            return View(events);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ownerId,id,name,details")] Events events)
        {
            if (ModelState.IsValid)
            {
                db.Entry(events).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ownerId = new SelectList(db.User, "id", "role", events.ownerId);
            return View(events);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Events events = db.Events.Find(id);
            db.Events.Remove(events);
            db.SaveChanges();
            return RedirectToAction("Index");
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
