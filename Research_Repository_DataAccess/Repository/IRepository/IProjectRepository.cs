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
    public interface IProjectRepository : IRepository<Project>
    {
        public ProjectVM GetProjectVM();

        bool HasItems(int id);

        Team GetTeam(int teamId);

        void Update(Project obj);

    }
}
