using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using System.Collections.Generic;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface IThemeRepository : IRepository<Theme>
    {
        IList<CheckboxVM> GetTagCheckboxes(int? themeId);

        IEnumerable<Tag> GetTags();

        IEnumerable<SelectListItem> GetTagList(IList<Tag> tags, bool useDb);

        ThemeObjectVM CreateThemeVM(int newId, string selectedThemeName = "", string selectedThemeDescription = "", int? id = null);

        void UpdateThemeTagsList(ThemeObjectVM themeVM);

        IList<Tag> UpdateTagsDb(IList<Tag> tempTags);

        IList<int> GetThemeIds(IEnumerable<Theme> themes);

        void Update(Theme obj);
    }
}
