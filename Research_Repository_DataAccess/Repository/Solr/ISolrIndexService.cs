using Research_Repository_Models.Models.Solr;
using System.Collections.Generic;

namespace Research_Repository_DataAccess.Repository.Solr
{
    public interface ISolrIndexService<T>
    {
        bool AddUpdate(T document);

        bool Delete(T document);

        bool DeleteAll();

        bool AddAll(IList<T> documents);

        bool Reindex(IList<T> documentList);

        ItemQueryResponse<T> FilterItems(ItemQueryParams itemQueryParams);
    }
}
