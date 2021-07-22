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
        ThemeVM CreateThemeVM(int? id);

        void UpdateThemeTagsList(ThemeVM themeVM);

        bool HasItems(int id);

        void Update(Theme obj);

    }
}
