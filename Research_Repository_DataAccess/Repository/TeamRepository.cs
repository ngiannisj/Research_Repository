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

        public IList<Project> GetTeamProjectsFromDb(int? id)
        {
            IList<Project> assignedProjects = new List<Project>();
            if (id != null)
            {
                assignedProjects = _db.Projects.AsNoTracking().Where(i => i.TeamId == id).ToList();
            }
            return assignedProjects;
        }

        public IList<Project> GetProjectsFromTeams(IList<Team> teams)
        {
            IList<Project> tempProjectList = new List<Project>();
            if (teams != null && teams.Count() > 0)
            {
                foreach (Team team in teams)
                {
                    if (team.Projects != null && team.Projects.Count() > 0)
                    {
                        foreach (Project project in team.Projects)
                        {
                            tempProjectList.Add(project);
                        }
                    }
                }
            }
            return tempProjectList;
        }

        //Check if a team has linked projects
        public bool HasProjects(int id, IList<Project> projects)
        {
            if (projects.FirstOrDefault(i => i.TeamId == id) != null)
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

        public IList<int> GetProjectIds(IList<Team> teams, bool fromDb)
        {
            IList<int> projectIds = new List<int>();
            if (fromDb == true)
            {
                projectIds = _db.Projects.Select(u => u.Id).ToList();
            } else if(teams != null && teams.Count() > 0)
            {
                foreach( Team team in teams)
                {
                    if (team.Projects != null && team.Projects.Count() > 0)
                    {
                        foreach (Project project in team.Projects)
                        {
                            projectIds.Add(project.Id);
                        }
                    }
                }
            }
            return projectIds;
        }

        public void UpsertProjects(int teamId, IList<Project> projects)
        {
            if (projects != null && projects.Count() > 0)
            {
                IList<int> dbProjectIdList = GetProjectIds(null, true);
                foreach (Project project in projects)
                {
                    Project dbProject = _db.Projects.AsNoTracking().FirstOrDefault(u => u.Id == project.Id);
                    int? dbProjectTeamId = 0;
                    if(dbProject != null)
                    {
                        dbProjectTeamId = dbProject.TeamId;
                    }
                    if (!dbProjectIdList.Contains(project.Id))
                    {
                        //If project does not exist in db
                        project.Id = 0;
                        _db.Projects.Add(project);
                        _db.SaveChanges();
                    } 
                    else if(dbProjectIdList.Contains(project.Id) && dbProjectTeamId != teamId)
                    {
                        //If project exists in db but team id is different
                        project.TeamId = teamId;
                        _db.Projects.Attach(project);
                    }

                }
            }
            _db.SaveChanges();
        }

        public void DeleteProjects(IList<int> tempProjectIds)
        {
            IList<Project> dbProjects = _db.Projects.AsNoTracking().ToList();
            foreach (Project project in dbProjects)
            {
                if(!tempProjectIds.Contains(project.Id))
                {
                    _db.Remove(project);
                }
            }
            _db.SaveChanges();
        }

        //Get dropdown list of all tags
        public IEnumerable<SelectListItem> GetTeamsList(IEnumerable<Team> teams)
        {
            IEnumerable<SelectListItem> teamSelectList = teams.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();

            return teamSelectList;
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
