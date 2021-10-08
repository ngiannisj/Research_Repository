using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Research_Repository_Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Contact { get; set; }

        public IList<Item> Items { get; set; }

        public IList<Project> Projects { get; set; }

        public IList<ApplicationUser> Users { get; set; }
    }
}
