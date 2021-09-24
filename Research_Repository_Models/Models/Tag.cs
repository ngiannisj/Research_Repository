using System.Collections.Generic;

namespace Research_Repository_Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ItemTag> ItemTags  { get; set; }
        public ICollection<ThemeTag> ThemeTags { get; set; }
    }
}
