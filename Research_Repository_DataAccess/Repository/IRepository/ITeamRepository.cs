using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository_Models;
using System.Collections.Generic;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface ITeamRepository : IRepository<Team>
    {
        IList<Project> GetTeamProjectsFromDb(int? id);
        IList<Project> GetProjectsFromTeams(IList<Team> teams);
        void UpsertProjects(int teamId, IList<Project> projects);
        void DeleteProjects(IList<int> tempProjectIds, bool deleteAllProjects);
        IList<int> GetTeamIds(IEnumerable<Team> teams);
        IList<int> GetProjectIds(IList<Team> teams, bool fromDb);
        IEnumerable<SelectListItem> GetTeamsList(IEnumerable<Team> teams);
        void Update(Team obj);
        void Attach(Team obj);
    }
}
