using System;
using System.IO;
using System.Web.Mvc;
using SupFile2.Utilities;
using SupFile2.Models;
using System.Linq;

namespace SupFile2.Controllers.Viewer
{
    [RoutePrefix("DisplayMovie")]
    public class DisplayMovieController : Controller
    {
        [Route("Index")]
        // GET: DisplayMovie
        public ActionResult Index(string name)
        {
            var chemin = (string)MySession.GetChemin();

            FileInfo file = new FileInfo(Path.Combine(Debug.LocalStorage(), name));

            if (chemin != null)
            {
                string fileURL = Path.Combine(Debug.LocalStorage(), chemin.ToString(), name);
                file = new FileInfo(fileURL);

            }
            if (file.Exists)
            {
                if (ViewerModel.ExtensionMovie.ToList().Contains(file.Extension.ToLower()))
                {
                    DisplayContent displayContent = new DisplayContent();

                    displayContent.ProcessStart(file.ToString());
                }

            }

            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }
    }
}