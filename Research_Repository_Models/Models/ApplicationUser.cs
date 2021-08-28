using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        [Display(Name = "Type Type")]
        public int? TeamId { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        public string Role { get; set; }

        public IList<Item> Items { get; set; }

    }
}
