using SupFile2.Entities;
using SupFile2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SupFile2.Controllers
{
    public class LandingPageController : Controller
    {
        // GET: LandingPage
        public ActionResult Index()
        {
            User user = (User)MySession.GetUser();
            ViewBag.User = user;
            return View();
        }
    }
}