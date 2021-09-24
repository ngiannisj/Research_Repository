using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using System.Collections.Generic;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        public ItemVM GetItemVM(int? id);

        public void UpdateItemTagsList(ItemVM itemVM);

        public ItemListVM GetItemListVM(string filterType, string checkedCheckbox, string searchText);

        public IList<int> GetAssignedProjects(IList<int> teamIds);

        public IList<int> GetAssignedTags(IList<int> themeIds);

        void Update(Item obj);
    }
}
