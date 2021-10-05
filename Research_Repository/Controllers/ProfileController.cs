using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using System.Collections.Generic;

namespace Research_Repository.Controllers
{
    [Authorize(Roles = WC.AllRoles)]
    public class ProfileController : Controller
    {

        private readonly IProfileRepository _profileRepo;
        private readonly ITeamRepository _teamRepo;
        private readonly UserManager<IdentityUser> _userManager; //Used for accessing current user properties
        public ProfileController(IProfileRepository profileRepo, ITeamRepository teamRepo, UserManager<IdentityUser> userManager)
        {
            _profileRepo = profileRepo;
            _teamRepo = teamRepo;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //Create dropdown selectlist from teams
            IEnumerable<SelectListItem> teamsSelectList = _teamRepo.GetTeamsList(null);
            //Update teams dropdown selectlist in tempData
            TempData.Put(WC.TempDataTeamSelectList, teamsSelectList);

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
