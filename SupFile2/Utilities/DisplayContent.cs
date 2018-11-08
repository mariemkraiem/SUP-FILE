using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupFile2.Utilities
{
    public class DisplayContent
    {
        public void ProcessStart(string chemin)
        {
            try
            {
              System.Diagnostics.Process.Start(chemin.ToString());
            }

            catch (Exception e)
            {
               //Popup message d'erreur à faire 
            }
        }
    }
}