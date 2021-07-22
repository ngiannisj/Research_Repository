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
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        private readonly ApplicationDbContext _db;

        public TeamRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }

        public bool HasProjects(int id)
        {
            if (_db.Projects.FirstOrDefault(i => i.TeamId == id) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(Team obj)
        {
            _db.Teams.Update(obj);
        }
    }
}
