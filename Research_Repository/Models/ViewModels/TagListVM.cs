using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository.Models.ViewModels
{
    public class TagListVM
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public bool CheckedState { get; set; }
    }
}
