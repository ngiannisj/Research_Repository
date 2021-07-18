﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Display(Name ="Theme Type")]
        public int ThemeId { get; set; }

        [ForeignKey("ThemeId")]
        public virtual Theme Theme { get; set; }

        public ICollection<ItemTag> ItemTags { get; set; }
    }
}