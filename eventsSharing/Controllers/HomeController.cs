using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eventsSharing.Controllers
{
    public class HomeController : Controller
    {
        private eventDBEntities db = new eventDBEntities();

        public ActionResult Index()
        {
            ViewBag.role = new SelectList(db.User, "role", "role");
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(User user)
        {
            db.User.Add(user);
            db.GetValidationErrors();
            db.SaveChanges();

            string message = string.Empty;
            switch (user.id)
            {
                case -1:
                    message = "Username already exists.\\nPlease choose a different username.";
                    break;
                case -2:
                    message = "Supplied email address has already been used.";
                    break;
                default:
                    message = "Registration successful.\\nUser Id: " + user.name.ToString() + user.id.ToString();
                    break;
            }

            ViewBag.Message = message;
            ViewBag.role = new SelectList(db.User, "role", "role");
            return View(user);
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            ViewBag.Message = "unknown";
            var registeredUser = db.User.Where(a => a.email == user.email).FirstOrDefault();
    
            
            if(registeredUser != null && registeredUser.password == user.password)
            {
                ViewBag.Message = "registered";
                HttpContext.Session["userId"] = registeredUser.id;
                HttpContext.Session["role"] = registeredUser.role;

                return RedirectToAction("Index", "Events");
            }

            return View(user);
        }



    }
}