﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootcampCapstone.Queries;
using System.Web.Security;
using System.Net;
using System.Net.Mail;

namespace BootcampCapstone.Controllers
{
    public class HomeController : Controller
    {

        private EventManagerEntities db = new EventManagerEntities();

        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (User.Identity.Name != "")
            {
                return RedirectToAction("Index", "Event", new { myEvents = "false" });
            }
            return View();
        }

        //Login Function

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BootcampCapstone.Models.AccountModel.LogInModel lm)
        {
            UserQueries q = new UserQueries();
            UserQueries.VerificationResult r = q.VerifyUsernameAndPassword(lm.UserName, lm.Password);
            switch (r)
            {
                case UserQueries.VerificationResult.UserNameDoesNotExist:
                case UserQueries.VerificationResult.PasswordIncorrect:
                    ViewBag.Error = "Either password or username is incorrect.";
                    break;
                case UserQueries.VerificationResult.Correct:

                    var authTicket = new FormsAuthenticationTicket(
                    1,                             // version
                    lm.UserName,                      // user name
                    DateTime.Now,                  // created
                    DateTime.Now.AddMinutes(10),   // expires
                    false,                    // persistent?
                    "Moderator;Admin"                        // can be used to store roles
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                    return RedirectToAction("Index", "Event");
            }
            return View();
        }

        public ActionResult LostPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(BootcampCapstone.Models.AccountModel.LostPasswordModel lostPassModel)
        {
            string enteredUsername = lostPassModel.UserName;
            var user = db.Users.FirstOrDefault(i => i.username.ToUpper() == enteredUsername.ToUpper());
            if (user != null)
            {
            }
            else
            {
                ViewBag.Error = "Entered email address is not registered.";
            }
            return View();
        }

        public ActionResult Logout()
        {
            Response.Cookies.Clear();
            FormsAuthentication.SignOut();
            HttpCookie c = new HttpCookie(User.Identity.Name);
            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c);

            Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
