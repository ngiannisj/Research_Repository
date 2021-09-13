﻿using System;
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

        //Solr fields
        public const string SolrId = "id";
        public const string SolrTitle = "title_tti";
        public const string SolrAbstract = "abstract_tti";
        public const string SolrCollaborator = "collaborator_tti";
        public const string SolrDateRange = "dateRange";
        public const string SolrTheme = "theme_tti";
        public const string SolrProject = "project_tti";
        public const string SolrTeam = "team_tti";
        public const string SolrTags = "tags_tti";
        public const string SolrKeyInsights = "keyInsights_tti";
        public const string SolrMethodology = "methodology_tti";
        public const string SolrLimitations = "limitations_tti";
        public const string SolrApprovedUse = "approvedUse";
        public const string SolrSensitivity = "sensitivity";
        public const string SolrNotifyUploader = "notifyUploader";
        public const string SolrNotifyLibrarian = "notifyLibrarian";
        public const string SolrStatus = "status";
        public const string SolrUploaderId = "userId";
        public const string SolrUploader = "uploader_tti";
        public const string SolrLastUpdatedDate = "lastUpdatedDate";

        //Date formats
        public const string YearMonthDay = "yyyy-MM-dd";
        public const string YearMonthDayTagHelper = "{0:yyyy-MM-dd}";

        //Pagination
        public const int NumOfItemsPerPage = 2; //Remember to change in solr javascript file as well
    }
}
