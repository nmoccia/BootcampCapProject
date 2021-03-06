﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace BootcampCapstone.Controllers
{
    [Authorize]
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

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string myEvents, string dateFrom, string dateTo)
        {

            // ViewBag initialization every time the page loads
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleParam = String.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
            ViewBag.StartDateParam = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewBag.EndDateParam = sortOrder == "EndDate" ? "EndDate_desc" : "EndDate";
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            TempData["CurrentPage"] = myEvents == "true" ? "MyEvents" : "FindEvents";

            // Database elements
            var userId = (from i in db.Users.Where(i => i.username == User.Identity.Name) select i.userID).First();
            var registrations = from i in db.Registrations select i;
            var eventIds = registrations.Where(i => i.userID == userId).Select(j => j.eventID).ToList();
            var events = from s in db.Events select s;
            ViewBag.EventSignedUpList = eventIds;
            ViewBag.EventOwnerList = events.Where(i => i.ownerID == userId).Select(j => j.eventID).ToList();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            // Event filtering
            
            if (!String.IsNullOrEmpty(dateFrom))
            {
                DateTime d = Convert.ToDateTime(dateFrom);
                events = (from i in events
                          where
                          DateTime.Compare(i.startDate, d) >= 0
                          select i);
            }
            if (!String.IsNullOrEmpty(dateTo))
            {
                DateTime d = Convert.ToDateTime(dateTo);
                events = (from i in events
                          where
                          DateTime.Compare(i.endDate, d) <= 0
                          select i);
            }
           
            if (myEvents == "true")
                events = events.Where(i => eventIds.Contains(i.eventID));
            
            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(i => i.title.ToUpper().Contains(searchString)
                            || i.location.ToUpper().Contains(searchString)
                            || i.eventDescription.ToUpper().Contains(searchString)
                            || i.Type.type1.ToUpper().Contains(searchString));
            }

            var users = from s in db.Users select s;

            switch (sortOrder)
            {
                case "EndDate":
                    events = events.OrderBy(s => s.endDate);
                    break;
                case "EndDate_desc":
                    events = events.OrderByDescending(s => s.endDate);
                    break;
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

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(events.ToPagedList(pageNumber, pageSize));
        }

        
        public ActionResult SignUp(int id = 0)
        {
            var registrations = from i in db.Registrations select i;
            registrations = registrations.Where(i => i.eventID == id);
            var Users = from i in db.Users select i;
            int userId = Users.First(i => i.username == User.Identity.Name).userID;
            if (registrations.Where(i => i.userID == userId).FirstOrDefault() == null)
            {
                BootcampCapstone.Registration reg = new Registration();
                reg.eventID = id;
                reg.userID = db.Users.First(i => i.username == User.Identity.Name).userID;
                db.Registrations.Add(reg);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        public ActionResult Withdraw(int id = 0)
        {
            var registration = db.Registrations.Where(i => i.eventID == id && i.User.username == User.Identity.Name).FirstOrDefault();
            if (registration != null)
            {
                db.Registrations.Remove(registration);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
            

        }

        public ActionResult MyEvents()
        {
            return RedirectToAction("Index", new {myEvents = "true" });
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
            TempData["CurrentPage"] = "CreateEvent";
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
                ev.ownerID = db.Users.Where(i => i.username == User.Identity.Name).Select(j => j.userID).FirstOrDefault();
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

            var userId = db.Users.First(i => i.username == User.Identity.Name).userID;
            if (db.Events.Find(id).ownerID != userId)
                return RedirectToAction("Index");

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
            var userId = db.Users.First(i => i.username == User.Identity.Name).userID;
            if (db.Events.Find(id).ownerID != userId)
                return RedirectToAction("Index");

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