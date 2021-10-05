using Research_Repository_Models.Models.Solr;
using Research_Repository_Utility;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;

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

        public bool DeleteAll()
        {
            try
            {
                _solr.Delete(_solr.Query(SolrQuery.All));
                _solr.Commit();
                return true;
            }
            catch (SolrNetException)
            {
                //Log exception
                return false;
            }
        }

        public bool AddAll(IList<T> documentList)
        {
            try
            {
                foreach (T document in documentList)
                {
                    _solr.Add(document);
                }
                _solr.Commit();
                return true;
            }
            catch (SolrNetException)
            {
                //Log exception
                return false;
            }
        }

        public bool Reindex(IList<T> documentList)
        {
            try
            {
                _solr.Delete(_solr.Query(SolrQuery.All));

                foreach (T document in documentList)
                {
                    _solr.Add(document);
                }
                _solr.Commit();
                return true;
            }
            catch (SolrNetException)
            {
                //Log exception
                return false;
            }
        }

        public ItemQueryResponse<T> FilterItems(ItemQueryParams itemQueryParams)
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
            DateTime startDate = DateTime.ParseExact("0001-01-01", WC.YearMonthDay, CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), WC.YearMonthDay, CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(itemQueryParams.StartDate) && DateHelper.CheckDate(itemQueryParams.StartDate))
            {
              startDate = DateTime.ParseExact(itemQueryParams.StartDate, WC.YearMonthDay, CultureInfo.InvariantCulture);
            }
            if(!string.IsNullOrEmpty(itemQueryParams.EndDate) && DateHelper.CheckDate(itemQueryParams.EndDate))
            {
               endDate = DateTime.ParseExact(itemQueryParams.EndDate, WC.YearMonthDay, CultureInfo.InvariantCulture);
            }


            SolrQueryResults<T> items = _solr.Query(query, new QueryOptions
            {
                FilterQueries = new ISolrQuery[] {
                    new SolrQueryInList(itemQueryParams.Themes != null && itemQueryParams.Themes.Count > 0 ? WC.SolrTheme : null, itemQueryParams.Themes),
                    new SolrQueryInList(itemQueryParams.Teams != null && itemQueryParams.Teams.Count > 0 ? WC.SolrTeam : null, itemQueryParams.Teams),
                    new SolrQueryInList(itemQueryParams.Projects != null && itemQueryParams.Projects.Count > 0 ? WC.SolrProject : null, itemQueryParams.Projects),
                    new SolrQueryInList(itemQueryParams.Tags != null && itemQueryParams.Tags.Count > 0 ? WC.SolrTags : null, itemQueryParams.Tags),
                    new SolrQueryInList(itemQueryParams.Sensitivity != null && itemQueryParams.Sensitivity.Count > 0 ? WC.SolrSensitivity : null, itemQueryParams.Sensitivity),
                    new SolrQueryInList(itemQueryParams.Approvals != null && itemQueryParams.Approvals.Count > 0 ? WC.SolrApprovedUse : null, itemQueryParams.Approvals),
                    new SolrQueryInList(itemQueryParams.Status != null && itemQueryParams.Status.Count > 0 ? WC.SolrStatus : null, itemQueryParams.Status),
                    new SolrQueryInList(itemQueryParams.UploaderId != null && itemQueryParams.UploaderId.Count > 0 ? WC.SolrUploaderId : null, itemQueryParams.UploaderId),
                    new SolrQueryByRange<DateTime>(WC.SolrDateRange, startDate, endDate)
                },
                Fields = new[] { WC.SolrId, WC.SolrTitle, WC.SolrTeam, WC.SolrAbstract, WC.SolrTags, WC.SolrNotifyUploader, WC.SolrUploader, WC.SolrStatus, WC.SolrLastUpdatedDate }, //Fields returned from solr
                OrderBy = new[] { new SortOrder(itemQueryParams.SearchText == null || itemQueryParams.SearchText == "" ? WC.SolrDateCreated : "score", Order.DESC), SortOrder.Parse(itemQueryParams.SearchText == null || itemQueryParams.SearchText == "" ? WC.SolrDateCreated : "score" + " desc") }, //If no search text filter is provided as a parameter, sort by score, else sort by the date the item was created
                StartOrCursor = new StartOrCursor.Start(Int32.Parse(itemQueryParams.PaginationStartItem)), //What item pagination starts from
                Rows = WC.NumOfItemsPerPage //How many items are returned for pagination
            });

            //Get total number of items returned from query (Not just number of items per pagination page)
            ItemQueryResponse<T> itemResponse = new ItemQueryResponse<T> { Items = items, NumOfTotalResults = items.NumFound };

            return itemResponse;
        }
    }
}