using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using System.Collections.Generic;
using System.Linq;

namespace Research_Repository.Controllers
{
    [Authorize(Roles = WC.LibrarianRole)]
    public class TeamController : Controller
    {

        private readonly ITeamRepository _teamRepo;

        public TeamController(ITeamRepository teamRepo)
        {
            _teamRepo = teamRepo;
        }

        public IActionResult Index(bool redirect)
        {
            //Create a team view model to pass to the view
            TeamVM teamVM = new TeamVM { Teams = null, NewTeamName = "" };

            //If the teams page is loading for the first time without making any changes
            if (redirect == false)
            {
                //Get list of all teams from the database, with project navigational property
                IList<Team> dbTeams = _teamRepo.GetAll(isTracking: false, include: i => i.Include(a => a.Projects)).ToList();

                //Create and save teams dropdown select list to temp data
                SaveTeamsState(dbTeams);

                //Update the team view model with teams from the database
                teamVM.Teams = dbTeams;
            }

            //If the teams page is reloading after changes have been made to teams or projects
            else
            {
                //Get list of updated teams from session
                IList<Team> tempTeams = HttpContext.Session.Get<IList<Team>>(WC.SessionTeams);

                //Create an empty list of projects for each team if a list does not already exist
                foreach (Team team in tempTeams)
                {
                    if (team.Projects == null)
                    {
                        team.Projects = new List<Project>();
                    }
                }
                //Create and save teams dropdown select list to temp data
                SaveTeamsState(tempTeams);

                //Solves error where inputs in the view display the incorrect values
                ModelState.Clear();

                //Update the team view model with teams from 'tempTeams'
                teamVM.Teams = tempTeams;
            }

            return View(teamVM);
        }

        //Save updated teams/projects to database
        public IActionResult SaveTeams(IList<Team> teams)
        {

            //Get list of teams stored in database
            IList<Team> dbTeamList = _teamRepo.GetAll(isTracking: false, include: i => i.Include(a => a.Projects)).ToList();
            //Get list of team ids stored in database
            IList<int> dbTeamIdList = _teamRepo.GetTeamIds(dbTeamList);

            //If there are no teams in the list, delete all remaining projects from database (precautionary clean up)
            if (teams == null || teams.Count == 0)
            {
                _teamRepo.DeleteProjects(null, true);
            }


            foreach (Team team in teams)
            {
                //If database does not contain a team return from the view, add it to the database
                if (!dbTeamIdList.Contains(team.Id))
                {
                    //Set team id to 0 to ensure it will be given a new value when added to the database
                    team.Id = 0;
                    //Get list of projects before team is added to the database so that they can be attributed to the id of the team after it is added to the database
                    IList<Project> teamProjects = team.Projects;
                    //Remove all projects from the team
                    team.Projects = null;
                    //Add team to the database
                    _teamRepo.Add(team);
                    _teamRepo.Save();
                    //Add back the projects to the team
                    team.Projects = teamProjects;

                    //Update dbTeamList and dbTeamIdList with updated database values after adding the new item
                    dbTeamList = _teamRepo.GetAll(isTracking: false, include: i => i.Include(a => a.Projects)).ToList();
                    dbTeamIdList = _teamRepo.GetTeamIds(dbTeamList);
                }

                //If database does contain a team returned from the view, update the values in the database
                if (dbTeamIdList.Contains(team.Id))
                {
                    //Get list of projects from the team
                    IList<Project> projects = team.Projects;

                    //Update the team's projects in the database
                    _teamRepo.UpsertProjects(team.Id, projects);

                    //Get list of project ids from all teams returned from the view
                    IList<int> tempProjectIdList = _teamRepo.GetProjectIds(teams, false);
                    //Remove projects from db if they do not exist in temp data
                    _teamRepo.DeleteProjects(tempProjectIdList, false);

                    //Update a team in the database
                    _teamRepo.Update(team);
                    _teamRepo.Save();

                    //Update dbTeamList and dbTeamIdList with updated database values after adding the new item
                    dbTeamList = _teamRepo.GetAll(isTracking: false, include: i => i.Include(a => a.Projects)).ToList();
                    dbTeamIdList = _teamRepo.GetTeamIds(dbTeamList);
                }
            }

            //Get list of team ids from teams list returned from view
            IList<int> tempTeamIdList = _teamRepo.GetTeamIds(teams);

            //If dbTeamList is not empty or null
            if(dbTeamList != null && dbTeamList.Count > 0)
            {
                //Delete any teams from the database which do not exist in the teams list returned from the view (This will recursively delete any projects belonging to this team)
                foreach (Team dbTeam in dbTeamList)
                {
                    if (!tempTeamIdList.Contains(dbTeam.Id))
                    {
                        _teamRepo.Remove(dbTeam);
                    }
                }
            }

            //Save changes to the database
            _teamRepo.Save();
            //Solves error where inputs in the view display the incorrect values
            ModelState.Clear();
            return RedirectToAction(nameof(Index));
        }

        //Add team to temp teams in session
        public IActionResult AddTeam(TeamVM teamVM)
        {
            //Generate new unique id for the new team
            int newId = 1;
            //If teams exist in the teams list returned from the view, find the largest id number and add 1 to it to find the next unique id number
            if (teamVM.Teams != null && teamVM.Teams.Count > 0)
            {
                newId = teamVM.Teams.Select(u => u.Id).ToList().Max() + 1;
            }
            //Instantiate an empty teams list if a list does not exist
            if (teamVM.Teams == null)
            {
                teamVM.Teams = new List<Team>();
            }
            //Add the new team to the list of teams returned from the view
            teamVM.Teams.Add(new Team { Id = newId, Name = teamVM.NewTeamName, Projects = new List<Project>() });

            //Solves error where inputs in the view display the incorrect values
            ModelState.Clear();

            //Save updated teams list to session and update the teams dropdown selectlist
            SaveTeamsState(teamVM.Teams);

            //Reload teams page with new teams list
            return RedirectToAction(nameof(Index), new { redirect = true });
        }

        //Delete team from temp teams in session
        public IActionResult DeleteTeam(TeamVM teamVM, int deleteId)
        {
            if (teamVM.Teams != null && teamVM.Teams.Count > 0)
            {
                //Get item to be removed from items list
                Team itemToRemove = teamVM.Teams.FirstOrDefault(u => u.Id == deleteId);
                //Remove the item if it exists
                if (itemToRemove != null)
                {
                    teamVM.Teams.Remove(itemToRemove);
                }
                //Solves error where inputs in the view display the incorrect values
                ModelState.Clear();
                //Update teams dropdown selectlist and temp teams stored in session
                SaveTeamsState(teamVM.Teams);
            }

            //Reload teams page with new teams list
            return RedirectToAction(nameof(Index), new { redirect = true });
        }


        public IEnumerable<SelectListItem> SaveTeamsState([FromBody] IList<Team> teams)
        {
            //Remove the 'Team' property from project to avoid infinite nesting, this allows the team to be stringified and uploaded to the session
            if (teams != null && teams.Count > 0)
            {
                foreach (Team team in teams)
                {
                    if (team.Projects != null && team.Projects.Count > 0)
                    {
                        foreach (Project project in team.Projects)
                        {
                            project.Team = null;
                        }
                    }
                }
            }

            //Create dropdown selectlist from teams
            IEnumerable<SelectListItem> teamsSelectList = _teamRepo.GetTeamsList(teams);
            //Update teams dropdown selectlist in tempData
            TempData.Put(WC.TempDataTeamSelectList, teamsSelectList);

            //Update teams list in session
            HttpContext.Session.Set(WC.SessionTeams, teams);

            return teamsSelectList;
        }

    }
}
