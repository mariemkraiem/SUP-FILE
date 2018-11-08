using MySql.Data.MySqlClient;
using SupFile2.Models;
using SupFile2.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SupFile2.Entities
{
    public class FileSystemItem
    {
        private static List<FileSystemItem> items = new List<FileSystemItem>();

        public int Id;
        public int UserId;
        public string Name;
        public string Path;
        public int ParentId;
        public bool IsDir;
        public string Drive;
        public long Size;
        public bool Shared;
        
        public string Extension
        {
            get
            {
                if (IsDir) return "";
                return Name.Split('.').Last();
            }
        }

        public string Filename
        {
            get
            {
                string[] elements = Name.Split('.');
                if (elements.Length == 1) return Name;
                else return Id.ToString() + "." + elements.Last();
            }
        }
        public string FilePath
        {
            get
            {
                if (IsDir) return null;
                string path = Drive + Database.StoragePath;
                path = System.IO.Path.Combine(path, UserId.ToString());
                path = System.IO.Path.Combine(path, Filename);
                return path;
            }
        }

        public FileSystemItem() { }
        //*************************** Delete ***********************
        public bool Delete() // TODO penser au partage
        {
            if (IsDir)
            {
                List<FileSystemItem> childs = Getchilds();
                foreach (FileSystemItem child in childs)
                {
                    child.Delete();
                }
            }
            else
            {
                string extension = "";
                string[] elements = Name.Split('.');
                if (elements.Length > 1)
                    extension = elements.Last();

                string path = "";
                path = Drive + Database.StoragePath;
                path = System.IO.Path.Combine(path, UserId.ToString());
                path = System.IO.Path.Combine(path, Id.ToString() + "." + extension);

                path = FilePath;
                FileInfo file = new FileInfo(path);
                if (!file.Exists)
                    return false;

                File.Delete(path);
                // TODO libérer de la mémoire
            }

            items.Remove(this); // Remove from the cache
            string sql = "DELETE FROM `filesystem` WHERE `id` = @id";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            return true;
        }
        //********************** Getchilds*********************************
        public List<FileSystemItem> Getchilds()
        {
            List<FileSystemItem> items = new List<FileSystemItem>();

            string sql = "SELECT `id`, `name`, `userId`, `isDir`, `drive`,`size`,`shared` FROM `filesystem` WHERE `parent` = " + Id + ";";
            MySqlCommand cmd = Database.Instance.Connection.CreateCommand();
            cmd.CommandText = sql;
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                FileSystemItem fsi = new FileSystemItem();
                fsi.Id = reader.GetInt32("id");
                fsi.UserId = reader.GetInt32("userId");
                fsi.Name = reader.GetString("name");
                fsi.Path = Path + "/" + fsi.Name;
                fsi.ParentId = Id;
                fsi.IsDir = reader.GetInt16("isDir") == 1 ? true : false;
                fsi.Shared = reader.GetInt16("shared") == 1 ? true : false;

                if (!fsi.IsDir) fsi.Drive = reader.GetString("drive");
                else fsi.Drive = null;

                fsi.Size = fsi.IsDir ? 0 : reader.GetInt32("size");
                fsi.Shared = reader.GetInt16("shared") == 1 ? true : false;

                // Remove old item from the cache to update it
                FileSystemItem oldFsi = FileSystemItem.items.Find(i => i.Id == fsi.Id);
                if (oldFsi != null)
                    FileSystemItem.items.Remove(oldFsi);

                FileSystemItem.items.Add(fsi);
                items.Add(fsi);
            }
            reader.Close();
            cmd.Dispose();
            return items;
        }
        //*************************************Getelement*****************************
        public static FileSystemItem GetElement(string path, int userId)
        {

            FileSystemItem directory = null;

            if (path == null)
            {
                directory = new FileSystemItem();
                directory.Id = 0;
                directory.UserId = userId;
                directory.Name = null;
                directory.ParentId = -1;
                directory.IsDir = true;
                directory.Size = 0;
                directory.Shared = false;
                return directory;
            }

            if (path[0] == '/') path = path.Substring(1);
            if (path.Last() == '/') path = path.Remove(path.Length - 1);

            // If final element already in cache, don't fetch it from db
            directory = items.Find(d => d.Path == path && d.UserId == userId);
            if (directory != null)
                return directory;

            int parent = 0;
            string currentPath = "";
            string[] elements = path.Split('/');

            foreach (string element in elements)
            {

                currentPath += '/' + element;
                if (currentPath[0] == '/') currentPath = currentPath.Substring(1);
                // If already in cache, don't fetch it from db
                directory = items.Find(d => d.Path == currentPath && d.UserId == userId);
                if (directory != null)
                    continue;

                string sql = "SELECT `id`, `isDir`, `drive`, `size`,`shared` FROM `FileSystem` WHERE `parent` = " + parent + " AND `name` = '" + element + "';";
                MySqlCommand cmd = Database.Instance.Connection.CreateCommand();
                cmd.CommandText = sql;
                MySqlDataReader reader = cmd.ExecuteReader();

                bool result = reader.Read();
                if (!result)
                {
                    reader.Close();
                    cmd.Dispose();
                    return null;
                }


                directory = new FileSystemItem();
                directory.Id = reader.GetInt32("id");
                directory.UserId = userId;
                directory.Name = element;
                directory.ParentId = parent;
                directory.Path = currentPath;
                directory.IsDir = reader.GetInt16("isDir") == 1 ? true : false;
                directory.Size = reader.GetInt32("size");
                directory.Shared = reader.GetInt16("shared") == 1 ? true : false;
                if (!directory.IsDir) directory.Drive = reader.GetString("drive");
                else directory.Drive = null;

                reader.Close();
                cmd.Dispose();
                items.Add(directory);
                parent = directory.Id;
            }

            return directory;
        }
        //*************************************create element***********************************
        public FileSystemItem createElement(string name, bool isDir, long size)
        {
            name = name.Replace("/", "");
            string path = Path + "/" + name;
            string driveName = null;
            
            if (path[0] == '/') path = path.Substring(1);
            if (!isDir)
            {
                //driveName = StorageManager.Instance.GetLeastUsedDriveName();
                driveName = StorageManager.Instance.GetLeastUsedStorage();
            }

            FileSystemItem child = GetElement(path, UserId);
            if (child != null) return null;

            child = new FileSystemItem();
            child.UserId = UserId;
            child.Name = name;
            child.Path = path;
            child.ParentId = Id;
            child.IsDir = isDir;
            child.Drive = driveName;
            child.Size = size;
            child.Shared = false;

            string sql = "Insert Into `FileSystem` (`userId`, `name`, `parent` ,`isDir`,`drive`,`size`,`shared`) values (@userId, @name, @parent, @isdir,@drive,@size,@shared)";

            MySqlCommand cmd = Database.Instance.Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@userId", child.UserId);
            cmd.Parameters.AddWithValue("@name", child.Name);
            cmd.Parameters.AddWithValue("@parent", child.ParentId);
            cmd.Parameters.AddWithValue("@isdir", child.IsDir);
            cmd.Parameters.AddWithValue("@drive", child.Drive);
            cmd.Parameters.AddWithValue("@size", child.Size);
            cmd.Parameters.AddWithValue("@shared", child.Shared);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new MySqlCommand("SELECT LAST_INSERT_ID();", Database.Instance.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();

            child.Id = reader.GetInt32(0);
            items.Add(child);

            reader.Close();
            cmd.Dispose();

            return child;
        }
        //************************************create directory******************************
        public bool createDirectory(string name)
        {
            FileSystemItem result = createElement(name, true,0);
            if (result != null)
                return true;
            return false;
        }
        //************************************create file**********************************
        public bool CreateFile(HttpPostedFileBase file)
        {
            User user = (User)MySession.GetUser();
            if (user.ajouterStockage(file.ContentLength))
            {
                FileSystemItem result = createElement(file.FileName, false, file.ContentLength);
                if (result != null)
                {
                    String[] words = file.FileName.Split('.');
                    string fileName = result.Id + "." + words[words.Length - 1];
                    string path = "";
                    path = result.Drive + Database.StoragePath;
                    path = System.IO.Path.Combine(path, UserId.ToString());
                    CreateDir(path);
                    path = System.IO.Path.Combine(path, fileName);
                    file.SaveAs(path);
                    user.Save();
                    return true;
                }
            }
            

            return false;
        }
        //*****************************************rename**************************************
        public bool Rename(string name)
        {
            string oldPath = Path;
            if (Id == 0) return false;

            string path = "";
            string[] elements = Path.Split('/');
            if (elements.Length == 1) path = name;
            else
            {
                for (int i = 0; i < elements.Length - 1; ++i)
                {
                    path += elements[i] + "/";
                }
                path += name;
            }

            if (path[0] == '/') path = path.Substring(1);

            FileSystemItem newName = GetElement(path, UserId);
            if (newName != null)
                return false;

            Name = name;
            Path = path;

            string sql = "UPDATE `FileSystem` SET `name` = @nouveauNom WHERE `id` = @id";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.Parameters.AddWithValue("@nouveauNom", Name);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            ClearCache(oldPath);

            return true;
        }

        public void SetShare(bool shared)
        {
            Shared = shared;

            int sh = shared ? 1 : 0;
            string sql = $"UPDATE `filesystem` SET `shared` = {sh} WHERE `id` = {Id}";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        //***************************************clear cache***************************************
        public void ClearCache(string regexPattern)
        {
            Regex rx = new Regex(regexPattern, RegexOptions.IgnoreCase);
            List<FileSystemItem> itemsToclear = items.FindAll(i => i.UserId == UserId && rx.IsMatch(i.Path));
            foreach (FileSystemItem itemToClear in itemsToclear)
            {
                items.Remove(itemToClear);
            }
        }
        //****************************************** tostring*********************************************
        public override string ToString()
        {
            return Path;
        }
        //*****************************************create dir*************************
        private void CreateDir(string path)
        {
            string[] elements = path.Split('\\');
            if (elements.Length <= 1) return;
            path = elements[0] + '\\';
            for (int i = 1; i < elements.Length; ++i)
            {
                path += elements[i];
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists) Directory.CreateDirectory(path);
                path += '\\';
            }
        }
    }
}