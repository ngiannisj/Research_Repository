using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        public ItemVM GetItemVM(int? id);

        public void UpdateItemTagsList(ItemVM itemVM);

        public ICollection<int> GetAssignedProjects(int id);

        public ICollection<int> GetAssignedTags(int id);

        void Update(Item obj);
    }
}
