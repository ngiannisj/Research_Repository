using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models.ViewModels
{
    public class ThemeVM
    {
        public Theme Theme { get; set; }

        public IEnumerable<SelectListItem> TagSelectList { get; set; }

        public IList<TagListVM> TagCheckboxes { get; set; }
    }
}
