using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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

        public ItemController(IItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
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
                itemVM.Item = _itemRepo.Find(id.GetValueOrDefault());
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

                if (itemVM.Item.Id == 0)
                {
                    //Creating
                    _itemRepo.Add(itemVM.Item);
                    _itemRepo.Save();
                    _itemRepo.UpdateItemTagsList(itemVM);
                }
                else
                {
                    //Updating

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
            _itemRepo.Remove(obj);
            _itemRepo.Save();
            return RedirectToAction("Index");
        }

        //GET - GETTHEMETAGS (AJAX CALL)
        public ICollection<int> GetThemeTags(int id)
        {
            return _itemRepo.GetAssignedTags(id);
        }
    }
}
