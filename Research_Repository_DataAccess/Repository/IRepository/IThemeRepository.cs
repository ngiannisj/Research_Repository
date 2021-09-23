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

        IEnumerable<Tag> GetTags();

        IEnumerable<SelectListItem> GetTagList(IList<Tag> tags, bool useDb);

        ThemeObjectVM CreateThemeVM(int newId, IList<ThemeObjectVM> themeVMs, string newThemeName = "", int? id = null);

        void UpdateThemeTagsList(ThemeObjectVM themeVM);

        void UpdateTagsDb(IList<ThemeObjectVM> tempThemes, IList<Tag> tempTags);

        bool HasItems(int id);

        IList<int> GetThemeIds(IEnumerable<Theme> themes);

        void Update(Theme obj);

    }
}
