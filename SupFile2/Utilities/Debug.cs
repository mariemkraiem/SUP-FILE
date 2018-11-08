using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupFile2.Utilities
{
    public class Debug
    {
        private static string path = "";

        public static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public static string LocalStorage()
        {
            if (path == "")
                path = System.IO.File.ReadAllLines("C:\\path.txt")[0];

            return path;
        }
    }
}