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

        public bool HasItems(int id)
        {
            if (_db.Items.FirstOrDefault(i => i.ProjectId == id) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Team GetTeam(int teamId)
        {
            Team team = _db.Teams.FirstOrDefault(u =>u.Id == teamId);
            return team;
        }

        public void Update(Project obj)
        {
            _db.Projects.Update(obj);
        }
    }
}
