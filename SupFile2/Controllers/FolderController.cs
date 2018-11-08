using System;
using System.IO;
using System.Web.Mvc;

namespace SupFile2.Controllers
{
    using SupFile2.Entities;
    using SupFile2.Models;
    using SupFile2.Utilities;
    using System.Configuration;
    using System.IO.Compression;
    using System.Web.UI;

    public class FolderController : Controller
    {

        // GET: Folder
        public ActionResult AddFolder()
        {
            var name = Request.QueryString["foldername"];
            User myUser = (User)MySession.GetUser();
            FileSystemItem fileSystemItem = FileSystemItem.GetElement((string)MySession.GetChemin(), myUser.Id);
            bool result = fileSystemItem.createDirectory(name);
            String status = "nok";
            if(result)
            {
                status = "ok";
            }
            return Content(status, "text/plain");
        }
        public ActionResult Delete(String name)
        {
            var chemin = (string)MySession.GetChemin();
            string newPath = (string)MySession.GetChemin() + "/" + name;
            FileSystemItem fileSystemItem = FileSystemItem.GetElement(newPath, MySession.GetUser().Id);
            bool result = fileSystemItem.Delete();
            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }
        public ActionResult Rename()
        {
            // TODO check if session user is null
            var name = Request.QueryString["foldername"];
            string oldname = Request.QueryString["folderoldname"];
            string status = "nok";
            string newPath = (string)MySession.GetChemin() +"/" + oldname;
            FileSystemItem fileSystemItem = FileSystemItem.GetElement(newPath, MySession.GetUser().Id);
            bool result = fileSystemItem.Rename(name);
            if(result)
            {
                status = "ok";
            }
            return Content(status, "text/plain");
        }

        public ActionResult Download(String name)
        {
            var chemin = (string)MySession.GetChemin();
            User myUser = (User)MySession.GetUser();
            FileSystemItem fileSystemItem = FileSystemItem.GetElement(name, myUser.Id);
            if (fileSystemItem != null)
            {
                string pathFolder = fileSystemItem.Path;
                string nameFolder = fileSystemItem.Name;
                string nameNoExt = nameFolder.Split('.')[0];

                string startPath = @pathFolder + "\\" + nameNoExt + ".txt";
                string zipPath = @pathFolder + "\\" + nameNoExt + ".zip";


                if (!string.IsNullOrEmpty(zipPath))
                {
                    FileInfo zipFile = new FileInfo(Path.Combine(Debug.LocalStorage(), nameNoExt + ".zip"));
                    if (chemin != null)
                    {
                        zipFile = new FileInfo(Path.Combine(Debug.LocalStorage(), chemin.ToString(), nameNoExt + ".zip"));
                    }
                    if (zipFile.Exists)
                        zipFile.Delete();

                    ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);

                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + nameFolder);

                    FileStream myFS = new FileStream(zipPath, FileMode.Open);
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


                    FileInfo zipFile2 = new FileInfo(Path.Combine(Debug.LocalStorage(), nameNoExt + ".zip"));
                    if (chemin != null)
                    {
                        zipFile2 = new FileInfo(Path.Combine(Debug.LocalStorage(), chemin.ToString(), nameNoExt + ".zip"));
                    }
                    if (zipFile2.Exists)
                        zipFile2.Delete();
                }
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
                    //fileSystemItem.Shared = false;
                    fileSystemItem.SetShare(false);
                }
                else
                {
                    //fileSystemItem.Shared = true;
                    fileSystemItem.SetShare(true);
                    string lien = "https://localhost:44326/Home/DisplayFile?chemin=%2F" + cheminFolder + "&userName=" + myUser.Id.ToString();
                    string content = "<p>" + lien + "</p><br><a href=\"/\">Retour</a>";
                    return Content(content, "text/html");
                }

                fileSystemItem.SetShare(fileSystemItem.Shared);
            }
            return RedirectToAction("Index", "Home", new { Chemin = chemin });
        }
    }
}