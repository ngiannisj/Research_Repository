using Research_Repository_Models;
using Research_Repository_Models.ViewModels;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface IItemRequestRepository : IRepository<Item>
    {
        public ItemRequestVM GetItemRequestVM();
    }
}
