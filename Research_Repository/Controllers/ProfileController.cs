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
    public class ProfileController : Controller
    {

        private readonly IProfileRepository _profileRepo;
        private readonly UserManager<IdentityUser> _userManager; //Used for accessing current user properties
        public ProfileController(IProfileRepository profileRepo, UserManager<IdentityUser> userManager)
        {
            _profileRepo = profileRepo;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ProfileVM profileVM = _profileRepo.GetProfileVM(_userManager, User);
            return View(profileVM);
        }


        //GET - UPSERT
        public IActionResult SaveProfile(ProfileVM profileVM)
        {
            //Update user field values in database
            string userId = _userManager.GetUserId(User);
            ApplicationUser user = _profileRepo.FirstOrDefault(u => u.Id == userId);
            user.FirstName = profileVM.User.FirstName;
            user.LastName = profileVM.User.LastName;
            user.TeamId = profileVM.User.TeamId;
            user.Role = profileVM.User.Role;
            _profileRepo.Attach(user); //Unlike update, 'attach' replaces only the changed fields instead of the whole record
            _profileRepo.Save();

            return RedirectToAction(nameof(Index));
        }

        //GET - USER ID (FROM AJAX CALL)
        public string GetUserId()
        {
            //Get id of current user from database
            return _userManager.GetUserId(User);
        }

    }
}
