using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
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

        public ItemController(IItemRepository itemRepo, IWebHostEnvironment webHostEnvironment)
        {
            _itemRepo = itemRepo;
            _webHostEnvironment = webHostEnvironment;
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
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (itemVM.Item.Id == 0)
                {

                    //Creating
                    _itemRepo.Add(itemVM.Item);
                    _itemRepo.Save();

                    string fileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\";
                    if (files.Count != 0)
                    {
                        itemVM.Item.Files = FileHelper.UploadFiles(files, webRootPath, fileLocation);
                    }

                    _itemRepo.UpdateItemTagsList(itemVM);
                }
                else
                {
                    string fileLocation = WC.ItemFilePath + itemVM.Item.Id + "\\";
                    //Updating
                    var objFromDb = _itemRepo.FirstOrDefault(filter: u => u.Id == itemVM.Item.Id, isTracking: false);

                    if (files.Count > 0)
                    {

                        if (objFromDb.Files != null)
                        {
                            List<string> filesArray = objFromDb.Files.Split(',').Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
                            foreach (string file in filesArray)
                            {
                                FileHelper.DeleteFile(webRootPath, file, fileLocation);
                            }
                            
                        }

                        itemVM.Item.Files = FileHelper.UploadFiles(files, webRootPath, fileLocation);
                    }
                    else
                    {
                        itemVM.Item.Files = objFromDb.Files;
                    }

                    _itemRepo.Update(itemVM.Item);
                    _itemRepo.UpdateItemTagsList(itemVM);
                }

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

                List<string> filesArray = obj.Files.Split(',').Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
                foreach (string file in filesArray)
                {
                    FileHelper.DeleteFile(webRootPath, file, fileLocation);
                }
            }
            _itemRepo.Remove(obj);
            _itemRepo.Save();
            return RedirectToAction("Index");
        }

        //GET - GETTEAMPROJECTS (AJAX CALL)
        public ICollection<int> GetTeamProjects(int id)
        {
            return _itemRepo.GetAssignedProjects(id);
        }

        //GET - GETTHEMETAGS (AJAX CALL)
        public ICollection<int> GetThemeTags(int id)
        {
            return _itemRepo.GetAssignedTags(id);
        }
    }
}
