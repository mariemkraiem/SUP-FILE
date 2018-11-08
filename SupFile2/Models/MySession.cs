using SupFile2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SupFile2.Models
{
    public static class MySession
    {
        public static void SetSession(string value, object myObj)
        {
            HttpContext.Current.Session[value] = myObj;
        }

        public static object GetSession(string value)
        {
            object myObj = null;

            if (!string.IsNullOrEmpty(value))
                myObj = HttpContext.Current.Session[value];

            return myObj;
        }


        public static object GetChemin()
        {
            return GetSession("Chemin");
        }

        public static void SetChemin(object myObj)
        {
            SetSession("Chemin", myObj);
        }

        public static User GetUser()
        {
            return (User) GetSession("User");
        }

        public static void SetUser(User user)
        {
            SetSession("User", user);
        }


    }
}