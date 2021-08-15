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
        IList<Project> GetTeamProjects(int? id);
        bool HasProjects(int id);
        IList<int> GetTeamIds(IEnumerable<Team> teams);
        void Update(Team obj);

    }
}
