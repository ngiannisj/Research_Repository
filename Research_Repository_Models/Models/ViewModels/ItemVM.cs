using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models.ViewModels
{
    public class ItemVM
    {
        public Item Item { get; set; }

        public IEnumerable<SelectListItem> ThemeSelectList { get; set; }

        public IList<TagListVM> TagList { get; set; }
    }
}
