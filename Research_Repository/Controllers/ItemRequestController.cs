using Microsoft.AspNetCore.Authorization;
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
        public ItemRequestController(IItemRequestRepository itemRequestRepo)
        {
            _itemRequestRepo = itemRequestRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<Item> itemRequests = _itemRequestRepo.GetAll(filter: i => i.Status != WC.Draft);
            return View(itemRequests);
        }

    }
}
