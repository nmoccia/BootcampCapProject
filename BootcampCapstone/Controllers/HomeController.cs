using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootcampCapstone.Queries;

namespace BootcampCapstone.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BootcampCapstone.Models.AccountModel.LogInModel lm)
        {
            UserQueries q = new UserQueries();
            UserQueries.VerificationResult r = q.VerifyUsernameAndPassword(lm.UserName, lm.Password);
            switch (r)
            {
                case UserQueries.VerificationResult.UserNameDoesNotExist:
                    ViewBag.Error = "Username does not exist";
                    break;
                case UserQueries.VerificationResult.PasswordIncorrect:
                    ViewBag.Error = "Password is incorrect";
                    break;
                case UserQueries.VerificationResult.Correct:
                    ViewBag.Error = "Correct!";
                    return RedirectToAction("Index", "User");
            }
            return View();
        }

    }
}
