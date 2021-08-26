using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using System.Threading.Tasks;

namespace Research_Repository.Controllers
{
    [Authorize(Roles = WC.LibrarianRole)]
    public class ItemController : Controller
    {

        private readonly IItemRepository _itemRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISolrIndexService<ItemSolr> _solr;
        public ItemController(IItemRepository itemRepo, IWebHostEnvironment webHostEnvironment, ISolrIndexService<ItemSolr> solr)
        {
            _itemRepo = itemRepo;
            _webHostEnvironment = webHostEnvironment;
            _solr = solr;

        }

        public IActionResult Index()
        {
            IEnumerable<Item> objList = _itemRepo.GetAll(includeProperties: WC.ThemeName);
            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {

            ItemVM itemVM = _itemRepo.GetItemVM(id);
            //Create new temp folder for files
            string webRootPath = _webHostEnvironment.WebRootPath;
            string targetFileLocation = WC.ItemFilePath + "temp\\";
            string sourceFileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\";
            FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, true);

            if (id == null)
            {
                //Creating
                return View(itemVM);
            }
            else
            {
                //Updating
                itemVM.Item = _itemRepo.FirstOrDefault(filter: i => i.Id == id, includeProperties: "Project");
                    itemVM.TeamId = itemVM.Item.Project.TeamId;

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
        public IActionResult Upsert(ItemVM itemVM)
        {

            if (ModelState.IsValid)
            {
                if(itemVM.SuggestedTagList != null && itemVM.SuggestedTagList.Count > 0)
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
                    //Creating
                    _itemRepo.Add(itemVM.Item);
                    _itemRepo.Save();
                    _solr.AddUpdate(new ItemSolr(itemVM.Item));

                    //Update files
                    string targetFileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\"; 
                    FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, false);

                    _itemRepo.UpdateItemTagsList(itemVM);
                }
                else
                {
                    //Updating
                    var objFromDb = _itemRepo.FirstOrDefault(filter: u => u.Id == itemVM.Item.Id, isTracking: false);

                    //Update files
                    string targetFileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\";
                    FileHelper.CopyFiles(null, webRootPath, sourceFileLocation, targetFileLocation, true);

                    _itemRepo.Update(itemVM.Item);
                    _itemRepo.UpdateItemTagsList(itemVM);
                }

                FileHelper.DeleteFiles(null, webRootPath, sourceFileLocation);
                _itemRepo.Save();
                return RedirectToAction("Index");
            }
            return View(itemVM);
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
                string fileLocation = WC.ItemFilePath + obj.Id + "\\";

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

        //GET - GETTEAMPROJECTS (FROM AJAX CALL)
        public ICollection<int> GetTeamProjects(int id)
        {
            return _itemRepo.GetAssignedProjects(id);
        }

        //GET - GETTHEMETAGS (FROM AJAX CALL)
        public ICollection<int> GetThemeTags(int id)
        {
            return _itemRepo.GetAssignedTags(id);
        }

        //GET - GETFILTEREDITEMS (FROM AJAX CALL)
        public SolrQueryResults<ItemSolr> GetFilteredItems(string itemQueryJson)
        {
            ItemQueryParams itemQueryParams = JsonConvert.DeserializeObject<ItemQueryParams>(itemQueryJson);
            return _solr.FilterItems(itemQueryParams);
        }
    }
}
