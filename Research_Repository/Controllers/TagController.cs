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
    public class TagController : Controller
    {

        private readonly ITagRepository _tagRepo;

        public TagController(ITagRepository tagRepo)
        {
            _tagRepo = tagRepo;
        }

        //POST - UPDATE
        public IEnumerable<Tag> UpdateTag(int id, string tagName, string actionName)
        {
            if (actionName == "Add" && id == 0)
            {
                Tag tag = new Tag
                {
                    Id = 0,
                    Name = tagName
                };
                _tagRepo.Add(tag);
            }
            else if (actionName == "Update" && id != 0)
            {
                Tag tag = _tagRepo.Find(id);
                tag.Name = tagName;
                _tagRepo.Update(tag);
            }
            else if (actionName == "Delete")
            {
                Tag tag = _tagRepo.Find(id);
                _tagRepo.Remove(tag);
            }

                _tagRepo.Save();

            return _tagRepo.GetAll();
            }

        //GET - GETTAGNAME (FROM AJAX CALL)
        public string GetTagName(int? id)
        {
            string tagName = "newTag";
            if (id != null)
            {
                tagName = _tagRepo.FirstOrDefault(i => i.Id == id, isTracking: false).Name;
            }
            return tagName;
        }
    }
}
