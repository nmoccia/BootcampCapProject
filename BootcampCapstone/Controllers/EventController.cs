using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace BootcampCapstone.Controllers
{
    public class EventController : Controller
    {

        public EventController()
        {

        }
        public EventController(BootcampCapstone.Queries.IEventQueries eventQueries)
        {
            _eventQueries = eventQueries;
        }

        private EventManagerEntities db = new EventManagerEntities();

        //
        // GET: /Event/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleParam = String.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
            ViewBag.StartDateParam = sortOrder == "Date" ? "Date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var events = from s in db.Events select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(i => i.title.ToUpper().Contains(searchString)
                            || i.location.ToUpper().Contains(searchString)
                            || i.eventDescription.ToUpper().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Title_desc":
                    events = events.OrderByDescending(s => s.title);
                    break;
                case "Date":
                    events = events.OrderBy(s => s.startDate);
                    break;
                case "Date_desc":
                    events = events.OrderByDescending(s => s.startDate);
                    break;
                default:
                    events = events.OrderBy(s => s.title);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(events.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Event/Details/5

        public ActionResult Details(int id = 0)
        {
            Event ev = db.Events.Find(id);
            if (ev == null)
            {
                return HttpNotFound();
            }
            return View(ev);
        }

        //
        // GET: /Event/Create

        public ActionResult Create()
        {
            ViewBag.categoryID = new SelectList(db.Categories, "categoryID", "category1");
            ViewBag.typeID = new SelectList(db.Types, "typeID", "type1");
            return View();
        }

        //
        // POST: /Event/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event ev)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(ev);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.categoryID = new SelectList(db.Categories, "categoryID", "category1", ev.categoryID);
            ViewBag.typeID = new SelectList(db.Types, "typeID", "type1", ev.typeID);
            return View(ev);
        }

        //
        // GET: /Event/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Event ev = db.Events.Find(id);
            if (ev == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryID = new SelectList(db.Categories, "categoryID", "category1", ev.categoryID);
            ViewBag.typeID = new SelectList(db.Types, "typeID", "type1", ev.typeID);
            return View(ev);
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event ev)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ev).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.categoryID = new SelectList(db.Categories, "categoryID", "category1", ev.categoryID);
            ViewBag.typeID = new SelectList(db.Types, "typeID", "type1", ev.typeID);
            return View(ev);
        }

        //
        // GET: /Event/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Event ev = db.Events.Find(id);
            if (ev == null)
            {
                return HttpNotFound();
            }
            return View(ev);
        }

        //
        // POST: /Event/Delete/5

        // Comment to test ability to commit and push easily

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event ev = db.Events.Find(id);
            db.Events.Remove(ev);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        /*
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        */
        private BootcampCapstone.Queries.IEventQueries _eventQueries;
        public const string TitleDescendingText = "title_desc";
    }
}