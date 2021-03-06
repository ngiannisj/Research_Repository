using System.Collections.Generic;

namespace Research_Repository_Models.ViewModels
{
    public class ItemListVM
    {
        public string SearchText { get; set; }

        public IList<CheckboxVM> TeamCheckboxes { get; set; }

        public IList<CheckboxVM> StatusCheckboxes { get; set; }

        public IList<CheckboxVM> ProjectCheckboxes { get; set; }

        public IList<CheckboxVM> ThemeCheckboxes { get; set; }

        public IList<CheckboxVM> TagCheckboxes { get; set; }

        public IList<CheckboxVM> ApprovalCheckboxes { set; get; }

        public IList<CheckboxVM> SensitivityCheckboxes { set; get; }
    }

}
