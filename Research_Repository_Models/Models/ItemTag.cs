﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository_Models
{
    public class ItemTag
    {
        public int ItemId { get; set; }
        public int TagId { get; set; }

        public Item Item { get; set; }
        public Tag Tag { get; set; }
    }
}