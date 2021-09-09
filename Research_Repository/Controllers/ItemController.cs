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
    public class ItemController : Controller
    {

        private readonly IItemRepository _itemRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager; //Used for accessing current user properties
        private readonly ISolrIndexService<ItemSolr> _solr;
        public ItemController(IItemRepository itemRepo, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, ISolrIndexService<ItemSolr> solr)
        {
            _itemRepo = itemRepo;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _solr = solr;

        }

        public IActionResult Index()
        {
            ItemListVM itemListVM = _itemRepo.GetItemListVM();
            return View(itemListVM);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {

            ItemVM itemVM = _itemRepo.GetItemVM(id);
            //Create new temp folder for files
            string webRootPath = _webHostEnvironment.WebRootPath;
            string targetFileLocation = WC.ItemFilePath + "temp\\";
            string sourceFileLocation = WC.ItemFilePath + id + "\\";
            FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, true);

            if (itemVM.Item.Status == WC.Submitted && itemVM.Item.NotifyLibrarian == true)
            {
                itemVM.Item.NotifyLibrarian = false;
                string librarianNotificationCount = _itemRepo.GetAll(isTracking: false).Select(i => i.NotifyLibrarian == true).Count().ToString();
                TempData.Put("librarianNotifications", librarianNotificationCount);
                _itemRepo.Update(itemVM.Item);
                _itemRepo.Save();
            }

            if (id == null)
            {
                //Creating
                return View(itemVM);
            }
            else
            {
                //Updating
                itemVM.Item = _itemRepo.FirstOrDefault(filter: i => i.Id == id, include: i => i.Include(a => a.Project));
                if (itemVM.Item.Project != null)
                {
                    itemVM.TeamId = itemVM.Item.Project.TeamId;
                }

                if (itemVM.Item == null)
                {
                    return NotFound();
                }
                return View(itemVM);
            }
        }


        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ItemVM itemVM, string submit)
        {
            itemVM.Item.LastUpdatedDate = DateTime.Today;

            if (User.IsInRole(WC.UploaderRole) && submit != "Submit" || itemVM.Item.Status == null || itemVM.Item.Status == "")
            {
                itemVM.Item.Status = WC.Draft;
            }
            else if (User.IsInRole(WC.UploaderRole) && submit == "Submit")
            {
                itemVM.Item.Status = WC.Submitted;
            }

            if (itemVM.SuggestedTagList != null && itemVM.SuggestedTagList.Count > 0)
            {
                itemVM.Item.SuggestedTags = string.Join("~~", itemVM.SuggestedTagList);
            }
            if (itemVM.KeyInsightsList != null && itemVM.KeyInsightsList.Count > 0)
            {
                itemVM.Item.KeyInsights = string.Join("~~", itemVM.KeyInsightsList);
            }

            //File parameters
            string sourceFileLocation = WC.ItemFilePath + "temp\\";
            string webRootPath = _webHostEnvironment.WebRootPath;

            if (itemVM.Item.Id == 0)
            {
                itemVM.Item.UploaderId = _userManager.GetUserId(User);

                //Update notifications
                if (itemVM.Item.Status == WC.Published)
                {
                    itemVM.Item.NotifyUploader = true;
                    string uploaderNotificationCount = _itemRepo.GetAll(isTracking: false).Select(i => i.NotifyUploader == true).Count().ToString();
                    TempData.Put("uploaderNotifications", uploaderNotificationCount);
                }
                else if (itemVM.Item.Status == WC.Submitted)
                {
                    itemVM.Item.NotifyLibrarian = true;
                    string librarianNotificationCount = _itemRepo.GetAll(isTracking: false).Select(i => i.NotifyLibrarian == true).Count().ToString();
                    TempData.Put("librarianNotifications", librarianNotificationCount);
                }
                else
                {
                    itemVM.Item.NotifyUploader = false;
                    itemVM.Item.NotifyLibrarian = false;
                }

                //Creating
                _itemRepo.Add(itemVM.Item);
                _itemRepo.Save();

                //Update files
                string targetFileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\";
                FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, false);

                _itemRepo.UpdateItemTagsList(itemVM);
            }
            else
            {

                //Update files
                string targetFileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\";
                FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, true);

                if (itemVM.Item.Status == WC.Published && _itemRepo.FirstOrDefault(i => i.Id == itemVM.Item.Id, isTracking: false).Status != WC.Published)
                {
                    itemVM.Item.NotifyUploader = true;
                    string uploaderNotificationCount = _itemRepo.GetAll(isTracking: false).Select(i => i.NotifyUploader == true).Count().ToString();
                    TempData.Put("uploaderNotifications", uploaderNotificationCount);
                }
                else if (itemVM.Item.Status == WC.Submitted && _itemRepo.FirstOrDefault(i => i.Id == itemVM.Item.Id, isTracking: false).Status != WC.Submitted)
                {
                    itemVM.Item.NotifyLibrarian = true;
                    string librarianNotificationCount = _itemRepo.GetAll(isTracking: false).Select(i => i.NotifyLibrarian == true).Count().ToString();
                    TempData.Put("librarianNotifications", librarianNotificationCount);
                }

                _itemRepo.Update(itemVM.Item);
                _itemRepo.UpdateItemTagsList(itemVM);
            }

            if (itemVM.Item.Status == WC.Rejected)
            {
                itemVM.Item.Comment = "";
            }
            FileHelper.DeleteFiles(null, webRootPath, sourceFileLocation);

            _itemRepo.Save();

            //Update solr
            if (itemVM.Item.Status == WC.Published)
            {
                //Get item from db to include navigation fields
                Item dbItem = _itemRepo.FirstOrDefault(u => u.Id == itemVM.Item.Id, isTracking: false, include: source => source
        .Include(a => a.Project)
        .ThenInclude(a => a.Team)
        .Include(a => a.ItemTags)
        .ThenInclude(a => a.Tag)
        .Include(a => a.Theme)
        .Include(a => a.Uploader));

                _solr.AddUpdate(new ItemSolr(dbItem));
            }

            return RedirectToAction("Index", "Profile");
        }

        //GET - VIEW
        public IActionResult View(int id)
        {
            Item item = _itemRepo.FirstOrDefault(u => u.Id == id, include: source => source
        .Include(a => a.Project)
        .ThenInclude(a => a.Team)
        .Include(a => a.ItemTags)
        .ThenInclude(a => a.Tag));

            if (item.NotifyUploader == true && item.UploaderId == _userManager.GetUserId(User))
            {
                item.NotifyUploader = false;
                string uploaderNotificationCount = _itemRepo.GetAll(isTracking: false).Select(i => i.NotifyUploader == true).Count().ToString();
                TempData.Put("uploaderNotifications", uploaderNotificationCount);
                _itemRepo.Update(item);
                _itemRepo.Save();
            }

            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }


        //DELETE - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _itemRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            if (obj.Files != null)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string fileLocation = WC.ItemFilePath + id + "\\";

                FileHelper.DeleteFiles(null, webRootPath, fileLocation);
            }
            _itemRepo.Remove(obj);
            _itemRepo.Save();
            return RedirectToAction("Index");
        }

        //POST - POSTFILES (FROM AJAX CALL)
        public string PostFiles()
        {
            var files = Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;
            string fileLocation = WC.ItemFilePath + "temp\\";

            return FileHelper.UploadFiles(files, webRootPath, fileLocation);
        }

        //DELETE - DELETEFILES (FROM AJAX CALL)
        public void DeleteFiles(string name)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string fileLocation = WC.ItemFilePath + "temp\\";

            FileHelper.DeleteFiles(name, webRootPath, fileLocation);
        }

        //GET - DOWNLOAD FILE
        public IActionResult GetDownloadedFile(string filePath)
        {
            return FileHelper.DownloadFile(filePath);
        }

        //GET - GETTEAMPROJECTS (FROM AJAX CALL)
        public ICollection<int> GetTeamProjects(IList<int> ids)
        {
            return _itemRepo.GetAssignedProjects(ids);
        }

        //GET - GETTHEMETAGS (FROM AJAX CALL)
        public ICollection<int> GetThemeTags(IList<int> ids)
        {
            return _itemRepo.GetAssignedTags(ids);
        }

        //GET - GETFILTEREDITEMS (FROM AJAX CALL)
        public SolrQueryResults<ItemSolr> GetFilteredItems(string itemQueryJson)
        {
            ItemQueryParams itemQueryParams = JsonConvert.DeserializeObject<ItemQueryParams>(itemQueryJson);
            return _solr.FilterItems(itemQueryParams);
        }
    }
}
