using System;
using System.IO;
using System.Web.Mvc;
using SupFile2.Utilities;
using SupFile2.Models;
using System.Linq;
using System.Text;
using System.Net;
using SupFile2.Entities;

namespace SupFile2.Controllers.Viewer
{
    [RoutePrefix("DisplayText")]
    public class DisplayTextController : Controller
    {
        [Route("Index")]
        // GET: DisplayText
        public ActionResult Index(String name)
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
                if (ViewerModel.ExtensionText.ToList().Contains(file.Extension.ToLower()))
                {
                    DisplayContent displayContent = new DisplayContent();

                    displayContent.ProcessStart(file.ToString());

                    /*FileSystemItem fileSystem = FileSystemItem.GetElement("test2/test.c", 1);*/                 
                }

            }
            return RedirectToAction("Index", "Home", new { Chemin = chemin });          
        }
    }
}