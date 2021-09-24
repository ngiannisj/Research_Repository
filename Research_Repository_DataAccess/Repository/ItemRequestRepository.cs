using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;

namespace Research_Repository_DataAccess.Repository
{
    public class ItemRequestRepository : Repository<Item>, IItemRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public ItemRequestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //Generate item request view model (Currently not needed to provide any data to view, however leaving here for future need)
        public ItemRequestVM GetItemRequestVM()
        {
            ItemRequestVM itemRequestVM = new ItemRequestVM();

            return itemRequestVM;
        }
    }
}
