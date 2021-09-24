using System.Collections.Generic;

namespace Research_Repository_Models.Models.Solr
{
    public class ItemQueryParams
    {
        public string SearchText { get; set; }
        public IList<string> Themes { get; set; }
        public IList<string> Teams { get; set; }
        public IList<string> Projects { get; set; }
        public IList<string> Tags { get; set; }
        public IList<string> Sensitivity { get; set; }
        public IList<string> Approvals { get; set; }
        public IList<string> Status { get; set; }
        public IList<string> UploaderId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PaginationStartItem { get; set; }
    }
}
