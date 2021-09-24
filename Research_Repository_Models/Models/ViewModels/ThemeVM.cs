using System.Collections.Generic;

namespace Research_Repository_Models.ViewModels
{
    public class ThemeVM
    {
        public IList<ThemeObjectVM> ThemeObjects { get; set; }

        public string NewThemeName { get; set; }
    }

    public class ThemeObjectVM
    {
        public Theme Theme { get; set; }

        public IList<CheckboxVM> TagCheckboxes { get; set; }
    }
}
