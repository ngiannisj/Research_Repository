using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ItemTag> ItemTags  { get; set; }
        public ICollection<ThemeTag> ThemeTags { get; set; }
    }
}
