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

       
        public IList<Project> GetTeamProjectsFromDb(int? teamId)
        {
            //Instantiate list of projects
            IList<Project> assignedProjects = new List<Project>();

            //Get projects with the foreign key of a team passed to the function
            if (teamId != null && teamId > 0)
            {
                assignedProjects = _db.Projects.AsNoTracking().Where(i => i.TeamId == teamId).ToList();
            }
            return assignedProjects;
        }

        //Get all projects from teams passed to the function
        public IList<Project> GetProjectsFromTeams(IList<Team> teams)
        {
            //Instantiate list of projets
            IList<Project> tempProjectList = new List<Project>();

            //If teams list has some items
            if (teams != null && teams.Count > 0)
            {
                foreach (Team team in teams)
                {
                    //If projects in the team is not empty
                    if (team.Projects != null && team.Projects.Count() > 0)
                    {
                        //Add all projects in the team to the 'tempProjectList'
                        foreach (Project project in team.Projects)
                        {
                            tempProjectList.Add(project);
                        }
                    }
                }
            }

            return tempProjectList;
        }

        //Get team ids from teams list passed to the function
        public IList<int> GetTeamIds(IEnumerable<Team> teams)
        {
            return teams.Select(u => u.Id).ToList();
        }

        //Get project Ids from teams list passed to function, or from projects in database if 'fromDb' is set to true
        public IList<int> GetProjectIds(IList<Team> teams, bool fromDb)
        {
            //Instantiate list of projectIds
            IList<int> projectIds = new List<int>();

            //If 'fromDb' is set to true, get project ids from database
            if (fromDb == true)
            {
                projectIds = _db.Projects.Select(u => u.Id).ToList();
            } 
            
            //If teams list is not empty
            else if(teams != null && teams.Count > 0)
            {
                //Get all project ids from all teams
                foreach( Team team in teams)
                {
                    if (team.Projects != null && team.Projects.Count > 0)
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

        //Add/Update projects in database
        public void UpsertProjects(int teamId, IList<Project> projects)
        {
            //If projects list passed from parameter is not empty
            if (projects != null && projects.Count > 0)
            {
                //Generate list of project ids
                IList<int> dbProjectIdList = GetProjectIds(null, true);

                foreach (Project project in projects)
                {
                    //Get project from database
                    Project dbProject = _db.Projects.AsNoTracking().FirstOrDefault(u => u.Id == project.Id);

                    //Get project team id
                    int? dbProjectTeamId = 0;
                    if(dbProject != null)
                    {
                        dbProjectTeamId = dbProject.TeamId;
                    }

                    //Add project to database if it does not already exist there
                    if (!dbProjectIdList.Contains(project.Id))
                    {
                        //If project does not exist in db
                        project.Id = 0;
                        _db.Projects.Add(project);
                    } 

                    //Update the project in the database if it already exists in ther
                    else
                    {
                        //If project exists in db but team id is different
                        project.TeamId = teamId;
                        _db.Projects.Update(project);
                    }

                }
            }

            _db.SaveChanges();
        }

        public void DeleteProjects(IList<int> tempProjectIds, bool deleteAllProjects)
        {
            //Get list of projects from database
            IList<Project> dbProjects = _db.Projects.AsNoTracking().ToList();

            //Delete all project in the database if 'deleteAllProjects' parameter is set to true (Called when no teams exist when saving teams)
            if(deleteAllProjects == true)
            {
                foreach (Project project in dbProjects)
                {
                        _db.Remove(project);
                }
            }

            //If tempProjectIds is not empty and 'deleteAllProjects' is set to false
            else if(deleteAllProjects == false && tempProjectIds != null && tempProjectIds.Count > 0)
            {
                //Delete selected ids from the database
                foreach (Project project in dbProjects)
                {
                    if (!tempProjectIds.Contains(project.Id))
                    {
                        _db.Remove(project);
                    }
                }
            }
           
            _db.SaveChanges();
        }

        //Get dropdown selectlist of all teams
        public IEnumerable<SelectListItem> GetTeamsList(IEnumerable<Team> teams)
        {
            //Use teams passed from parameter to generate a dropdown selectlist
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
