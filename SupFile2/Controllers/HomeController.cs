using System;
using System.IO;
using System.Web.Mvc;
using SupFile2.Utilities;

namespace SupFile2.Controllers
{
    using MySql.Data.MySqlClient;
    using SupFile2.Entities;
    using SupFile2.Models;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class HomeController : Controller
    {

        //[Route("Home/{*chemin}")]
        [Route("")]
        public ActionResult Index(String c, String u)
        {
            string chemin = Request.QueryString["chemin"];
            string userName = Request.QueryString["userName"];

            if (chemin == "DisplayFile")
            {
                return Redirect("DisplayFile");
            }
            User user = MySession.GetUser();
            if (user != null)
            {
                StorageManager sManager = StorageManager.Instance;
                var userStockageFree = user.StockageMax - user.Stockage;

                MySession.SetChemin(chemin);

                FileSystemItem fsi = FileSystemItem.GetElement(chemin, user.Id);
                if (!fsi.IsDir)
                {
                    return RedirectToAction("DisplayFile");
                }

                FileSystemItem[] items = fsi.Getchilds().ToArray();
                items = items.Where(i => i.UserId == user.Id).ToArray();
                ViewBag.User = user;
                ViewBag.Items = items;
                ViewBag.Chemin = chemin;
                ViewBag.UserStockage = sManager.KilobyteToGygabyte(userStockageFree);
                
                return View();
            }
            else if (!string.IsNullOrEmpty(chemin) && !string.IsNullOrEmpty(userName))
            {
                MySession.SetChemin(chemin);

                FileSystemItem fsi = FileSystemItem.GetElement(chemin, Int32.Parse(userName));
                FileSystemItem[] items = fsi.Getchilds().Where(i => i.Shared).ToArray();

                if (items != null && items.Count() > 0)
                {
                    ViewBag.Items = items;
                    ViewBag.Chemin = chemin;

                    return View();
                }
                else if (fsi.IsDir)
                {
                    ViewBag.Items = items;
                    ViewBag.Chemin = chemin;
                    return View();
                }
                else
                    return RedirectToAction("Index", "LandingPage");
            }
            else
            {
                return RedirectToAction("Index", "LandingPage");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /*public ActionResult DisplayFile(String chemin)
        {
            //string chemin = MySession.GetChemin() as string;
            FileSystemItem fsi = FileSystemItem.GetElement(chemin, MySession.GetUser().Id);
            ViewBag.Chemin = fsi.Filename;
            return View();*/

        public ActionResult DisplayFile()
        {
            string chemin = Request.QueryString["chemin"];
            int userId;
            if (Request.QueryString["userName"] == null)
                userId = MySession.GetUser().Id;
            else
                userId = int.Parse(Request.QueryString["userName"]);

            FileSystemItem fsi = FileSystemItem.GetElement(chemin, userId);
            bool logged = MySession.GetUser() != null;
            if (logged) logged = MySession.GetUser().Id == fsi.UserId;
            if (fsi != null && (fsi.Shared || logged))
            {
                ViewBag.Chemin = fsi.Path;
                ViewBag.FileName = fsi.Name;
                ViewBag.UserId = fsi.UserId;

                switch(fsi.Extension.ToLower())
                {
                    case "png":
                    case "jpeg":
                    case "jpg":
                        ViewBag.Type = "image";
                        break;
                    case "mp4":
                        ViewBag.Type = "video";
                        break;
                    case "mp3":
                        ViewBag.Type = "audio";
                        break;
                }

                if (ViewBag.Type == null) return RedirectToAction("Index", "LandingPage");
                return View();
            }
            return RedirectToAction("Index", "LandingPage");
        }

        public ActionResult GetFile()
        {
            string chemin = Request.QueryString["chemin"]; // TODO fix
            int userId;
            if (Request.QueryString["userName"] == null)
                userId = MySession.GetUser().Id;
            else
                userId = int.Parse(Request.QueryString["userName"]);

            FileSystemItem fsi = FileSystemItem.GetElement(chemin, userId);
            if (fsi == null) return RedirectToAction("Index", "LandingPage");
            
            FileInfo file = new FileInfo(fsi.FilePath);
            if (file != null)
            {
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fsi.Name);

                FileStream myFS = new FileStream(fsi.FilePath, FileMode.Open);
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
            return null;
        }

        public bool IsSharedFile(string id)
        {
            string sql = "SELECT COUNT(*) as exists FROM `share` WHERE ";
            sql += "`share.id` LIKE '" + id + ";";

            MySqlCommand command = Database.Instance.Connection.CreateCommand();
            command.CommandText = sql;
            MySqlDataReader reader = command.ExecuteReader();

            bool result = false;
            while (reader.Read())
            {
                result = reader.GetInt32("exists") > 0;
            }
            reader.Close();

            return result;
        }

    }
}