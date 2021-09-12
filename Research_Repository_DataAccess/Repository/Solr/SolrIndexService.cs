using Research_Repository_Models.Models.Solr;
using Research_Repository_Models.Solr;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                query = new SolrQuery($"indexed_text:{itemQueryParams.SearchText}");
            }
            else
            {
                query = SolrQuery.All;
            }

            //Convert string date to dateTime format
            DateTime startDate = DateTime.ParseExact(itemQueryParams.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(itemQueryParams.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            SolrQueryResults<T> items = _solr.Query(query, new QueryOptions
            {
                FilterQueries = new ISolrQuery[] {
                    new SolrQueryInList(itemQueryParams.Themes != null && itemQueryParams.Themes.Count() > 0 ? "theme_tti" : null, itemQueryParams.Themes),
                    new SolrQueryInList(itemQueryParams.Teams != null && itemQueryParams.Teams.Count() > 0 ? "team_tti" : null, itemQueryParams.Teams),
                    new SolrQueryInList(itemQueryParams.Projects != null && itemQueryParams.Projects.Count() > 0 ? "project_tti" : null, itemQueryParams.Projects),
                    new SolrQueryInList(itemQueryParams.Tags != null && itemQueryParams.Tags.Count() > 0 ? "tags_tti" : null, itemQueryParams.Tags),
                    new SolrQueryInList(itemQueryParams.Sensitivity != null && itemQueryParams.Sensitivity.Count() > 0 ? "sensitivity" : null, itemQueryParams.Sensitivity),
                    new SolrQueryInList(itemQueryParams.Approvals != null && itemQueryParams.Approvals.Count() > 0 ? "approvedUse" : null, itemQueryParams.Approvals),
                    new SolrQueryByRange<DateTime>("dateRange", startDate, endDate)
                },
                Fields = new[] { "id", "title_tti", "team_tti", "abstract_tti", "tags_tti" }, //Fields returned from solr
                OrderBy = new[] { new SortOrder("lastUpdatedDate", Order.DESC), SortOrder.Parse("lastUpdatedDate asc") },
                StartOrCursor = new StartOrCursor.Start(0), //Where pagination starts from (May need to make number a dynamic variable)
                Rows = 10 //How many items are returned for pagination
            });


            return items;
        }
    }
}
