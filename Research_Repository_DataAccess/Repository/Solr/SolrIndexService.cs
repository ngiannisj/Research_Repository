using Research_Repository_Models.Models.Solr;
using Research_Repository_Models.Solr;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository.Solr
{
    public class SolrIndexService<T, TSolrOperations> : ISolrIndexService<T>
        where TSolrOperations : ISolrOperations<T>
    {
        private readonly TSolrOperations _solr;
        public SolrIndexService(ISolrOperations<T> solr)
        {
            _solr = (TSolrOperations)solr;
        }
        public bool AddUpdate(T document)
        {
            try
            {
                // If the id already exists, the record is updated, otherwise added                         
                _solr.Add(document);
                _solr.Commit();
                return true;
            }
            catch (SolrNetException ex)
            {
                //Log exception
                return false;

            }
        }

        public bool Delete(T document)
        {
            try
            {
                //Can also delete by id                
                _solr.Delete(document);
                _solr.Commit();
                return true;
            }
            catch (SolrNetException)
            {
                //Log exception
                return false;
            }
        }

        public SolrQueryResults<T> FilterItems(ItemQueryParams itemQueryParams)
        {
            ISolrQuery query;

            if (!string.IsNullOrEmpty(itemQueryParams.SearchText))
            {
                query = new SolrQuery(itemQueryParams.SearchText);
            }
            else
            {
                query = SolrQuery.All;
            }

            //Convert string date to dateTime format
            DateTime startDate = DateTime.ParseExact(itemQueryParams.StartDate, "d", null);
            DateTime endDate = DateTime.ParseExact(itemQueryParams.EndDate, "d", null);

            SolrQueryResults<T> items = _solr.Query(query, new QueryOptions
            {
                FilterQueries = new ISolrQuery[] {
                    itemQueryParams.Themes[0] != "" ? new SolrQueryInList("themes", itemQueryParams.Themes) : null,
                    itemQueryParams.Teams[0] != "" ? new SolrQueryInList("teams", itemQueryParams.Teams) : null,
                    itemQueryParams.Projects[0] != "" ? new SolrQueryInList("projects", itemQueryParams.Projects) : null,
                    itemQueryParams.Tags[0] != "" ? new SolrQueryInList("tags",  itemQueryParams.Tags) : null,
                    itemQueryParams.Sensitivity[0] != "" ? new SolrQueryInList("sensitivity", itemQueryParams.Sensitivity) : null,
                    itemQueryParams.Approvals[0] != "" ? new SolrQueryInList("approvals", itemQueryParams.Approvals) : null,
                    startDate != endDate ? new SolrQueryByRange<DateTime>("dateRange", startDate, endDate): null,
                }
            });
            return items;
        }
    }
}
