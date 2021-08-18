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
    public interface ITeamRepository : IRepository<Team>
    {
        IList<Project> GetTeamProjectsFromDb(int? id);
        IList<Project> GetProjectsFromTeams(IList<Team> teams);
        void UpsertProjects(int teamId, IList<Project> projects);
        void DeleteProjects(IList<int> tempProjectIds);
        bool HasProjects(int id, IList<Project> projects);
        IList<int> GetTeamIds(IEnumerable<Team> teams);
        IList<int> GetProjectIds(IList<Team> teams, bool fromDb);
        IEnumerable<SelectListItem> GetTeamsList(IEnumerable<Team> teams);
        void Update(Team obj);
        void Attach(Team obj);
    }
}
