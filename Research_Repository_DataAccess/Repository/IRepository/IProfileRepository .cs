using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface IProfileRepository : IRepository<ApplicationUser>
    {
        IEnumerable<SelectListItem> GetTeamSelectList();

        ProfileVM GetProfileVM(UserManager<IdentityUser> userManager, ClaimsPrincipal userInstance);

        void Update(ApplicationUser obj);

        void Attach(ApplicationUser obj);
    }
}
