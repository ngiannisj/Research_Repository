using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Research_Repository_Models.ViewModels
{
    public class ProfileVM
    {
        public ApplicationUser User { get; set; }

        public IEnumerable<SelectListItem> TeamSelectList { get; set; }
    }

}
