using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository
{
    public class ItemRequestRepository : Repository<Item>, IItemRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public ItemRequestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public ItemRequestVM GetItemRequestVM()
        {

            ItemRequestVM itemRequestVM = new ItemRequestVM
            {
                Items = _db.Items.Where(i => i.Status != WC.Draft).ToList()
            };

            return itemRequestVM;
        }
    }
}
