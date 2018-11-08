using SupFile2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SupFile2.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            MySession.SetUser(null);
            return RedirectToAction("Index", "LandingPage");
            
        }
    }
}