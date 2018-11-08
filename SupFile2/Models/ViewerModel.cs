using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupFile2.Models
{
    public static class ViewerModel
    {
        private static string[] _ExtensionImage = new string[4] { ".jpeg", ".jpg", ".png", ".gif" };
        public static string[] ExtensionImage
        {
            get
            {
                return _ExtensionImage;
            }
            set
            {
                _ExtensionImage = value;
            }
        }

        private static string[] _ExtensionMovie = new string[3] { ".avi", ".mp4", ".mkv"};
        public static string[] ExtensionMovie
        {
            get
            {
                return _ExtensionMovie;
            }
            set
            {
                _ExtensionMovie = value;
            }
        }

        private static string[] _ExtensionText = new string[3] { ".docx", ".pdf", ".txt"};
        public static string[] ExtensionText
        {
            get
            {
                return _ExtensionText;
            }
            set
            {
                _ExtensionText = value;
            }
        }
    }
}