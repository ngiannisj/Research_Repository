using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Research_Repository_DataAccess.Repository
{
    public class ProfileRepository : Repository<ApplicationUser>, IProfileRepository
    {
        private readonly ApplicationDbContext _db;

        public ProfileRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //Generate a profile view model to pass to view
        public ProfileVM GetProfileVM(UserManager<IdentityUser> userManager, ClaimsPrincipal userInstance)
        {
            //Get current user id
            string userId = userManager.GetUserId(userInstance);

            //Get current user object
            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            //Get current users team details
            user.Team = _db.Teams.FirstOrDefault(u => u.Id == user.TeamId);

            //Get items created by current user
            user.Items = _db.Items.Where(u => u.UploaderId == userId).ToList();

            //Generate profile view model
            ProfileVM profileVM = new ProfileVM
            {
                User = user,
                TeamSelectList = GetTeamSelectList()
            };

            return profileVM;
        }

        //Get dropdown list of all teams
        public IEnumerable<SelectListItem> GetTeamSelectList()
        {
            //Get all teams from database
            IEnumerable<Team> teams = _db.Teams;

            //Generate select list for teams
            IEnumerable<SelectListItem> teamSelectList = teams.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();

            return teamSelectList;
        }

        public void Update(ApplicationUser obj)
        {
            _db.ApplicationUsers.Update(obj);
        }

        public void Attach(ApplicationUser obj)
        {
            _db.ApplicationUsers.Attach(obj);
        }
    }
}
