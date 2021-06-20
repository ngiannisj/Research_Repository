using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository.Models
{
    public class Theme
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
