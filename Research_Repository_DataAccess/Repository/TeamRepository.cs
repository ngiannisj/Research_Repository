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

        public IList<Project> GetTeamProjects(int? id)
        {
            IList<Project> assignedProjects = new List<Project>();
            if (id != null)
            {
                assignedProjects = _db.Projects.AsNoTracking().Where(i => i.TeamId == id).ToList();
            }
            return assignedProjects;
        }

        //Check if a team has linked projects
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

        public IList<int> GetTeamIds(IEnumerable<Team> teams)
        {
            return teams.Select(u => u.Id).ToList();
        }

        public void AddProjects(int teamId, IList<Project> projects)
        {
            if (projects != null && projects.Count() > 0)
            {
                IList<int> projectIdList = _db.Projects.Select(u => u.Id).ToList();
                foreach (Project project in projects)
                {
                    if(!projectIdList.Contains(project.Id))
                    {
                        project.Id = 0;
                        _db.Projects.Add(new Project { Name = project.Name, TeamId = teamId });
                    }
                }
            }
        }
        
        public void Update(Team obj)
        {
            _db.Teams.Update(obj);
        }

        public void Attach(Team obj)
        {
            _db.Teams.Attach(obj);
        }
    }
}
