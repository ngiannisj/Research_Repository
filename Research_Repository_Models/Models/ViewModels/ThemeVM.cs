using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
