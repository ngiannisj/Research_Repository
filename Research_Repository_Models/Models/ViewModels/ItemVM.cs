using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository_Models.Models.ViewModels;
using System.Collections.Generic;

namespace Research_Repository_Models.ViewModels
{
    public class ItemVM
    {
        public Item Item { get; set; }

        public int? TeamId { get; set; }

        public IList<string> SuggestedTagList { get; set; }
        
        public IList<string> KeyInsightsList { get; set; }

        public IEnumerable<SelectListItem> TeamSelectList { get; set; }

        public IEnumerable<SelectListItem> StatusSelectList { get; set; }

        public IEnumerable<SelectListItem> ProjectSelectList { get; set; }

        public IEnumerable<SelectListItem> ThemeSelectList { get; set; }

        public IList<CheckboxVM> TagList { get; set; }

        public IEnumerable<RadioButtonVM> ApprovalRadioButtons { set; get; }

        public IEnumerable<RadioButtonVM> SensitivityRadioButtons { set; get; }
    }

}
