using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_DataAccess.Repository.Solr;
using Research_Repository_Models;
using Research_Repository_Models.Solr;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using SolrNet;
using System.Collections.Generic;
using System.Linq;

namespace Research_Repository.Controllers
{
    [Authorize(Roles = WC.LibrarianRole)]
    public class ItemRequestController : Controller
    {

        private readonly IItemRequestRepository _itemRequestRepo;
        private readonly IItemRepository _itemRepo;
        private readonly ISolrCoreAdmin _solrAdmin; //For solr admin tasks such as reloading core, currently not implementing any solr core functionalities
        private readonly ISolrIndexService<ItemSolr> _solr;
        public ItemRequestController(IItemRequestRepository itemRequestRepo, IItemRepository itemRepo, ISolrCoreAdmin solrAdmin, ISolrIndexService<ItemSolr> solr)
        {
            _itemRequestRepo = itemRequestRepo;
            _itemRepo = itemRepo;
            _solrAdmin = solrAdmin;
            _solr = solr;
        }

        public IActionResult Index()
        {
            ItemRequestVM itemRequestVM = _itemRequestRepo.GetItemRequestVM();
            return View(itemRequestVM);
        }

        //Delete all existing solr indexes and add all solr items
        public bool ReindexItems()
        {
            //Get all items from database with navigational properties
            IList<Item> items = _itemRepo.GetAll(isTracking: false, include: source => source
    .Include(a => a.Project)
    .ThenInclude(a => a.Team)
    .Include(a => a.ItemTags)
    .ThenInclude(a => a.Tag)
    .Include(a => a.Theme)
    .Include(a => a.Uploader)).ToList();

            //Transform items list into solritems list so they can be indexed in solr
            IList<ItemSolr> solrItemsList = new List<ItemSolr>();
            foreach (Item item in items)
            {
                solrItemsList.Add(new ItemSolr(item));
            }

            //Delete all existing solr indexes and add all solr items

            if(_solr.Reindex(solrItemsList))
                {
                    return true;
                } else
                {
                    return false;
                }

        }
    }
}
