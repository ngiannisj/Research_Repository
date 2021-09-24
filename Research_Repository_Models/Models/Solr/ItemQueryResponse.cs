using SolrNet;

namespace Research_Repository_Models.Models.Solr
{
    public class ItemQueryResponse<T>
    {
        public SolrQueryResults<T> Items { get; set; }

        public int NumOfTotalResults { get; set; }
    }
}
