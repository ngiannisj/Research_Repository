using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository
{
    public class ProfileRepository : Repository<ApplicationUser>, IProfileRepository
    {
        private readonly ApplicationDbContext _db;

        public ProfileRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public ProfileVM GetProfileVM(UserManager<IdentityUser> userManager, ClaimsPrincipal userInstance)
        {
            string userId = userManager.GetUserId(userInstance);

            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            user.Team = _db.Teams.FirstOrDefault(u => u.Id == user.TeamId);
            user.Items = _db.Items.Where(u => u.UploaderId == userId).ToList();

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
            IEnumerable<Team> teams = _db.Teams;
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
