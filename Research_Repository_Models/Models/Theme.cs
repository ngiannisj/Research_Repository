using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Research_Repository_Models
{
    public class Theme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<ThemeTag> ThemeTags { get; set; }

        public ICollection<Item> Items { get; set; }
    }
}
