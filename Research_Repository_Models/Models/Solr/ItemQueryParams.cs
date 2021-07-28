using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_Models.Models.Solr
{
    public class ItemQueryParams
    {
        public string SearchText { get; set; }
        public IList<string> Teams { get; set; }
        public IList<string> Projects { get; set; }
        public IList<string> Tags { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
