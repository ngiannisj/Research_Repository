using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        public IList<Project> Projects { get; set; }

        public IList<ApplicationUser> Users { get; set; }
    }
}
