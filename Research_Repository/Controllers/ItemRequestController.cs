﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_DataAccess.Repository.Solr;
using Research_Repository_Models;
using Research_Repository_Models.Models.Solr;
using Research_Repository_Models.Solr;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using SolrNet;
using SolrNet.Commands.Cores;
using SolrNet.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public IActionResult ReindexItems()
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
            _solr.Reindex(solrItemsList);

            return RedirectToAction(nameof(Index));

        }
    }
}
