using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models.ViewModels
{
    public class ProjectVM
    {
        public Project Project { get; set; }

        public IEnumerable<SelectListItem> TeamSelectList { get; set; }
    }

}
