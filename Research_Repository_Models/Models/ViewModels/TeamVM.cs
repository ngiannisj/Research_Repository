using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models.ViewModels
{
    public class TeamVM
    {
        public IList<Team> Teams { get; set; }

        public string NewTeamName { get; set; }
    }
}
