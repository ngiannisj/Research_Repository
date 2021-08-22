using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface IThemeRepository : IRepository<Theme>
    {
        IList<CheckboxVM> GetTagCheckboxes(int? id, IList<CheckboxVM> tempTagCheckboxes);

        IEnumerable<SelectListItem> GetTagList();

        ThemeVM CreateThemeVM(int newId, IList<ThemeVM> themeVMs, int? id = null);

        void UpdateThemeTagsList(ThemeVM themeVM);

        void UpdateTagsDb(IList<ThemeVM> tempThemes);

        bool HasItems(int id);

        IList<int> GetThemeIds(IEnumerable<Theme> themes);

        void Update(Theme obj);

    }
}
