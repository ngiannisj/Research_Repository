using Research_Repository_Models.Models.Solr;
using Research_Repository_Models.Solr;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository.Solr
{
    public interface ISolrIndexService<T>
    {
        bool AddUpdate(T document);
        bool Delete(T document);

        SolrQueryResults<T> FilterItems(ItemQueryParams itemQueryParams);
    }
}
