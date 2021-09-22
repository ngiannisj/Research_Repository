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
        private readonly IWebHostEnvironment _webHostEnvironment; //Used for file upload
        private readonly UserManager<IdentityUser> _userManager; //Used for accessing current user properties
        private readonly ISolrIndexService<ItemSolr> _solr;
        public ItemController(IItemRepository itemRepo, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, ISolrIndexService<ItemSolr> solr)
        {
            _itemRepo = itemRepo;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _solr = solr;
        }

        public IActionResult Index(string filterType, string checkedCheckbox, string searchText)
        {
            ItemListVM itemListVM = _itemRepo.GetItemListVM(filterType, checkedCheckbox, searchText);
            return View(itemListVM);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {

            ItemVM itemVM = _itemRepo.GetItemVM(id);

            //Redirect to view page for item if the current user is an uploader not librarian
            if (id != null && User.IsInRole(WC.UploaderRole) && (itemVM.Item.Status == WC.Submitted || itemVM.Item.Status == WC.Published))
            {
                return RedirectToAction(nameof(View), id);
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string targetFileLocation = WC.ItemFilePath + "temp\\"; //Get new temp folder for temp files not yet added to item
            string sourceFileLocation = WC.ItemFilePath + id + "\\"; //Get item folder for files in this item
            FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, true); //Copy from temp folder to item folder

            //Update item notification status in database and solr index if the customer who created the item is viewing it
            if (itemVM.Item.NotifyUploader == true && itemVM.Item.UploaderId == _userManager.GetUserId(User))
            {
                itemVM.Item.NotifyUploader = false;
                _itemRepo.Update(itemVM.Item);
                _itemRepo.Save();

                //Get item from db to include navigation fields
                Item dbItem = _itemRepo.FirstOrDefault(u => u.Id == itemVM.Item.Id, isTracking: false, include: source => source
        .Include(a => a.Project)
        .ThenInclude(a => a.Team)
        .Include(a => a.ItemTags)
        .ThenInclude(a => a.Tag)
        .Include(a => a.Theme)
        .Include(a => a.Uploader));
                _solr.AddUpdate(new ItemSolr(itemVM.Item)); //Update solr
            }

            //If creating a new item
            if (id == null)
            {
                //Creating
                return View(itemVM);
            }
            else
            {
                //If updating an existing item
                itemVM.Item = _itemRepo.FirstOrDefault(filter: i => i.Id == id, include: i => i.Include(a => a.Project));
                if (itemVM.Item != null && itemVM.Item.Project != null)
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
            if (itemVM != null)
            {
                //Set lastUpdatedDate to todays date
                itemVM.Item.LastUpdatedDate = DateTime.Today;

                //Update item status
                if (User.IsInRole(WC.UploaderRole) && submit != "Submit" || itemVM.Item.Status == null || itemVM.Item.Status == "")
                {
                    itemVM.Item.Status = WC.Draft;
                }
                else if (User.IsInRole(WC.UploaderRole) && submit == "Submit")
                {
                    itemVM.Item.Status = WC.Submitted;
                }

                //Join all suggested tags into one string
                if (itemVM.SuggestedTagList != null && itemVM.SuggestedTagList.Count > 0)
                {
                    itemVM.Item.SuggestedTags = string.Join("~~", itemVM.SuggestedTagList);
                }
                //Join all key insights into one string
                if (itemVM.KeyInsightsList != null && itemVM.KeyInsightsList.Count > 0)
                {
                    itemVM.Item.KeyInsights = string.Join("~~", itemVM.KeyInsightsList);
                }

                //File parameters
                string sourceFileLocation = WC.ItemFilePath + "temp\\"; //Get temp file folder
                string webRootPath = _webHostEnvironment.WebRootPath;

                //If new item is being created
                if (itemVM.Item.Id == 0)
                {
                    itemVM.Item.UploaderId = _userManager.GetUserId(User); //Get current user

                    //Update notifications for items who created the item
                    if (itemVM.Item.Status == WC.Published || itemVM.Item.Status == WC.Rejected)
                    {
                        itemVM.Item.NotifyUploader = true;
                    }
                    else
                    {
                        itemVM.Item.NotifyUploader = false;
                    }

                    //Set date created to todays date
                    itemVM.Item.DateCreated = DateTime.Today;
                    //Add to database
                    _itemRepo.Add(itemVM.Item);
                    _itemRepo.Save();

                    //Copy files folder from temp files to item file folder
                    string targetFileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\";
                    FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, false);

                    //Update tags associated with a the item
                    _itemRepo.UpdateItemTagsList(itemVM);
                }
                else
                {
                    //Update files
                    string targetFileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\";
                    FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, true); //Copy files from temp folder to item folder

                    //Update notify uploader status
                    if (_itemRepo.FirstOrDefault(i => i.Id == itemVM.Item.Id, isTracking: false).Status != itemVM.Item.Status && itemVM.Item.Status != WC.Submitted)
                    {
                        itemVM.Item.NotifyUploader = true;
                    }

                    _itemRepo.Update(itemVM.Item);
                    _itemRepo.UpdateItemTagsList(itemVM);
                }

                //Reset comment field if status of item is not 'Rejected'
                if (itemVM.Item.Status != WC.Rejected)
                {
                    itemVM.Item.Comment = "";
                }

                FileHelper.DeleteFiles(null, webRootPath, sourceFileLocation); //Delete temp files folder

                _itemRepo.Save();

                //Get item from db to include navigation fields
                Item dbItem = _itemRepo.FirstOrDefault(u => u.Id == itemVM.Item.Id, isTracking: false, include: source => source
        .Include(a => a.Project)
        .ThenInclude(a => a.Team)
        .Include(a => a.ItemTags)
        .ThenInclude(a => a.Tag)
        .Include(a => a.Theme)
        .Include(a => a.Uploader));

                _solr.AddUpdate(new ItemSolr(dbItem)); //Update solr

                return RedirectToAction(nameof(Index), "Profile");
            } else
            {
                return NotFound();
            }
        }

        //GET - VIEW
        public IActionResult View(int id)
        {
            //Get item with navigation properties
            Item item = _itemRepo.FirstOrDefault(u => u.Id == id, include: source => source
        .Include(a => a.Project)
        .ThenInclude(a => a.Team)
        .Include(a => a.ItemTags)
        .ThenInclude(a => a.Tag)
        .Include(a => a.Theme)
        .Include(a => a.Uploader));

            //Update notification status to false if the current user is the user who created the item
            if (item.NotifyUploader == true && item.UploaderId == _userManager.GetUserId(User))
            {
                item.NotifyUploader = false;
                _itemRepo.Update(item);
                _itemRepo.Save();

                _solr.AddUpdate(new ItemSolr(item));
            }

            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }


        //DELETE - DELETE
        public void Delete(int? id)
        {
            var obj = _itemRepo.Find(id.GetValueOrDefault());
            if (obj.Files != null)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string fileLocation = WC.ItemFilePath + id + "\\";

                FileHelper.DeleteFiles(null, webRootPath, fileLocation); //Delete files associated with this item
            }
            //Delete from database
            _itemRepo.Remove(obj);
            _itemRepo.Save();

            //Get item from db to include navigation fields
            Item dbItem = _itemRepo.FirstOrDefault(u => u.Id == id, isTracking: false, include: source => source
    .Include(a => a.Project)
    .ThenInclude(a => a.Team)
    .Include(a => a.ItemTags)
    .ThenInclude(a => a.Tag)
    .Include(a => a.Theme)
    .Include(a => a.Uploader));

            _solr.Delete(new ItemSolr(dbItem)); //Delete solr index
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
        public ItemQueryResponse<ItemSolr> GetFilteredItems(string itemQueryJson)
        {
            ItemQueryParams itemQueryParams = JsonConvert.DeserializeObject<ItemQueryParams>(itemQueryJson);
            return _solr.FilterItems(itemQueryParams);
        }

        //SET - SESSION NOTIFICATION VALUE (FROM AJAX CALL)
        public IList<string> AddNotificationsToSession()
        {
            IList<string> notificationStatusList = _itemRepo.GetAll(filter: u => u.UploaderId == _userManager.GetUserId(User), isTracking: false).Where(u => u.NotifyUploader == true).Select(u => u.Status).ToList();

            return notificationStatusList;
        }

        //SET - SESSION ITEM REQUEST COUNT VALUES (FROM AJAX CALL)
        public int AddItemRequestCountToSession()
        {
            if (User.IsInRole(WC.LibrarianRole))
            {
                int itemRequestCount = _itemRepo.GetAll(filter: u => u.Status == WC.Submitted, isTracking: false).Count();
                return itemRequestCount;

            }else
            {
                return 0;
            }
            
        }
    }
}
