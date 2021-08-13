﻿using Microsoft.AspNetCore.Mvc;
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
        IList<TagListVM> GetTagList(int? id);

        ThemeVM CreateThemeVM(string newId, int? id=null);

        void UpdateThemeTagsList(ThemeVM themeVM);

        bool HasItems(int id);

        void Update(Theme obj);

    }
}
