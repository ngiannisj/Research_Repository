using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Research_Repository_Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ItemTag> ItemTags { get; set; }

        public ICollection<ThemeTag> ThemeTags { get; set; }
    }
}
