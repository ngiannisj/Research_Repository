using System.Collections.Generic;

namespace Research_Repository_Models.ViewModels
{
    public class HomeVM
    {
        public IList<Theme> Themes { get; set; }

        public IList<Team> Teams { get; set; }
    }
}
