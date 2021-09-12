using Newtonsoft.Json;
using Research_Repository_Models.Solr;
using SolrNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_Models.Models.Solr
{
    public class ItemQueryResponse<T>
    {
        public SolrQueryResults<T> Items { get; set; }
        public int NumOfTotalResults { get; set; }
    }
}
