using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Utility
{
    public static class WC
    {
        //File paths
        public const string ImagePath = @"\files\images\themes\";
        public const string ItemFilePath = @"\files\documents\items\";

        //Roles
        public const string LibrarianRole = "Librarian";
        public const string UploaderRole = "Uploader";

        //Database tables
        public const string ThemeName = "Theme";
        public const string ItemName = "Item";

        //Approved uses
        public const string Internal = "Internal";
        public const string External = "External";

        //Sensitivity
        public const string Unclassified = "Unclassified";
        public const string Protected = "Protected";

        //Item status
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Published = "Published";
        public const string Rejected = "Rejected";
    }
}
