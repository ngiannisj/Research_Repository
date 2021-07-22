using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ApplicationDbContext _db;

        public ProjectRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }


        public ProjectVM GetProjectVM()
        {
            ProjectVM projectVM = new ProjectVM()
            {
                Project = new Project(),
                TeamSelectList = _db.Teams.AsNoTracking().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            return projectVM;
        }

        public void Update(Project obj)
        {
            _db.Projects.Update(obj);
        }
    }
}
