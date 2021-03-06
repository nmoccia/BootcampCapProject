﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BootcampCapstone.Controllers
{
    public class UserController : Controller
    {
        private EventManagerEntities db = new EventManagerEntities();

        //
        // GET: /User/
       [Authorize]
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Food);
            return View(users.ToList());
        }

       [Authorize]
       public ActionResult MyAccount()
       {
           TempData["CurrentPage"] = "MyAccount";
           var userId = db.Users.Where(i => i.username.ToUpper() == User.Identity.Name.ToUpper()).Select(j => j.userID).FirstOrDefault();

           return RedirectToAction("Details", new { id = userId });
       }

        //
        // GET: /User/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            ViewBag.foodID = new SelectList(db.Foods, "foodID", "food1");
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {            
            if (ModelState.IsValid)
            {
                var authTicket = new FormsAuthenticationTicket(
                    1,                             // version
                    user.username,                      // user name
                    DateTime.Now,                  // created
                    DateTime.Now.AddMinutes(10),   // expires
                    false,                    // persistent?
                    "Moderator;Admin"                        // can be used to store roles
                    );

                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Event");
            }

            ViewBag.foodID = new SelectList(db.Foods, "foodID", "food1", user.foodID);
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(BootcampCapstone.Models.AccountModel.LogInModel lm)
        {
           
            return View(lm);
        }


        //
        // GET: /User/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            TempData["CurrentPage"] = "MyAccount";
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.foodID = new SelectList(db.Foods, "foodID", "food1", user.foodID);
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new {id = user.userID });
            }
            ViewBag.foodID = new SelectList(db.Foods, "foodID", "food1", user.foodID);
            return View(user);
        }

        //
        // GET: /User/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            TempData["CurrentPage"] = "MyAccount";
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Response.Cookies.Clear();
            FormsAuthentication.SignOut();
            HttpCookie c = new HttpCookie(User.Identity.Name);
            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c);

            Session.Clear();

            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}