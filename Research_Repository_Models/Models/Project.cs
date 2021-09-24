using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Research_Repository_Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Team Type")]
        public int? TeamId { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        public ICollection<Item> Items  { get; set; }
    }
}
