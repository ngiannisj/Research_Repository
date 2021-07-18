using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models
{
    public class ThemeTag
    {
        public int ThemeId { get; set; }
        public int TagId { get; set; }

        public Theme Theme { get; set; }
        public Tag Tag { get; set; }
    }
}
