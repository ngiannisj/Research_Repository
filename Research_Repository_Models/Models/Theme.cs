using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models
{
    public class Theme
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        public string Image { get; set; }

        public ICollection<ThemeTag> ThemeTags { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
