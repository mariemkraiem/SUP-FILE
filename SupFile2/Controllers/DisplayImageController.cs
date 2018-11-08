using System;
using System.IO;
using System.Web.Mvc;
using SupFile2.Utilities;
using System.Configuration;
using SupFile2.Models;
using System.Linq;
using System.Drawing;
using System.Web;
using System.Collections.Generic;
using SupFile2.Entities;

namespace SupFile2.Controllers.Viewer
{
    [RoutePrefix("DisplayImage")]
    public class DisplayImageController : Controller
    {
   
        public ActionResult Index(String name)
        {
            var chemin = (string)MySession.GetChemin();

            FileInfo file = new FileInfo(Path.Combine(Debug.LocalStorage(), name));
            FileSystemItem fsi = FileSystemItem.GetElement(chemin + "/" + name, MySession.GetUser().Id);
            DisplayContent displayContent = new DisplayContent();
            displayContent.ProcessStart(fsi.FilePath);

            /*if (chemin != null)
            {
                string fileURL = Path.Combine(Debug.LocalStorage(), chemin.ToString(), name);
                file = new FileInfo(fileURL);

            }
            if (file.Exists)
            {
                if (ViewerModel.ExtensionImage.ToList().Contains(file.Extension.ToLower()))
                {
                    DisplayContent displayContent = new DisplayContent();

                    displayContent.ProcessStart(file.ToString());

                }
            }*/

            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }
    }
}