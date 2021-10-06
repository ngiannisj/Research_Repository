using System.Collections.Generic;

namespace Research_Repository_Models.ViewModels
{
    public class TeamVM
    {
        public IList<Team> Teams { get; set; }

        public string SelectedTeamName { get; set; }

        public string SelectedTeamContact { get; set; }
    }
}
