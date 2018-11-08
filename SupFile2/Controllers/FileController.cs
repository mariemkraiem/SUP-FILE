using System;
using System.IO;
using System.Web.Mvc;
using SupFile2.Utilities;
using System.Configuration;
using SupFile2.Models;
using System.IO.Compression;
using SupFile2.Entities;
using System.Web;
using System.Linq;

namespace SupFile2.Controllers
{
    public class FileController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddFile()
        {
            var chemin = (string)MySession.GetChemin();
            HttpPostedFileBase file = Request.Files[0];
            //File will be saved in application root
            FileSystemItem fileSystemItem = FileSystemItem.GetElement((string)MySession.GetChemin(), MySession.GetUser().Id); // TODO check is session user is null
            bool result = fileSystemItem.CreateFile(file);

            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }


        public ActionResult Delete(String name)
        {
            var chemin = (string)MySession.GetChemin();
            string newPath = (string)MySession.GetChemin() + "/" + name;
            FileSystemItem fileSystemItem = FileSystemItem.GetElement(newPath, MySession.GetUser().Id); // TODO check if user is null
            bool result = fileSystemItem.Delete();
            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }
        public ActionResult Rename()
        {
            var name = Request.QueryString["filename"];
            string oldname = Request.QueryString["fileoldname"];
            string status = "nok";
            string newPath = (string)MySession.GetChemin() + "/" + oldname;
            FileSystemItem fileSystemItem = FileSystemItem.GetElement(newPath, MySession.GetUser().Id);
            bool result = fileSystemItem.Rename(name);
            if (result)
            {
                status = "ok";
            }
            return Content(status, "text/plain");
        }

        public ActionResult Download(String name)
        {
            var chemin = (string)MySession.GetChemin();
            FileSystemItem item = FileSystemItem.GetElement(chemin + "/" + name, MySession.GetUser().Id);
            FileInfo file = new FileInfo(Path.Combine(Debug.LocalStorage(), name));
            if (chemin != null)
            {
                file = new FileInfo(Path.Combine(Debug.LocalStorage(), chemin.ToString(), name));
            }
            if (file != null)
            {
                string pathFile = file.FullName;
                string nameFile = file.Name;
                string nameNoExt = nameFile.Split('.')[0];

                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + nameFile);

                FileStream myFS = new FileStream(item.FilePath, FileMode.Open);
                MemoryStream myMS = new MemoryStream();
                myMS.SetLength(myFS.Length);
                myFS.Read(myMS.GetBuffer(), 0, (int)myFS.Length);
                myFS.Close();
                myMS.Flush();

                Response.BinaryWrite(myMS.ToArray());
                Response.Flush();

                myMS.Dispose();
                myMS.Close();

                Response.End();
            }

            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }

        public ActionResult Share(String name)
        {
            var chemin = (string)MySession.GetChemin();
            User myUser = (User)MySession.GetUser();

            string cheminFolder = string.Empty;
            if (chemin != null)
                cheminFolder += (string)MySession.GetChemin();
            cheminFolder += name;

            FileSystemItem fileSystemItem = FileSystemItem.GetElement(cheminFolder, myUser.Id);
            if (fileSystemItem != null)
            {
                if (fileSystemItem.Shared)
                {
                    fileSystemItem.Shared = false;
                }
                else
                {
                    fileSystemItem.Shared = true;
                }

                fileSystemItem.SetShare(fileSystemItem.Shared);
            }
            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }

        public ActionResult DisplayContent(String name)
        {
            var chemin = (string)MySession.GetChemin();
            FileInfo file = new FileInfo(Path.Combine(Debug.LocalStorage(), name));

            if (chemin != null)
            {
                file = new FileInfo(Path.Combine(Debug.LocalStorage(), chemin.ToString(), name));
            }

            if (file.Exists)
            {
                if (ViewerModel.ExtensionImage.ToList().Contains(file.Extension.ToLower()))
                {
                    return RedirectToAction("Index", "DisplayImage", new { Name = name });
                }

                else if (ViewerModel.ExtensionMovie.ToList().Contains(file.Extension.ToLower()))
                {
                    return RedirectToAction("Index", "DisplayMovie", new { Name = name });
                }

                else if (ViewerModel.ExtensionText.ToList().Contains(file.Extension.ToLower()))
                {
                    return RedirectToAction("Index", "DisplayText", new { Name = name });
                }
            }
            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }
    }
}