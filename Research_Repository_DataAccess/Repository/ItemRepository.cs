using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        private readonly ApplicationDbContext _db;

        public ItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public ItemVM GetItemVM(int? id)
        {
            ICollection<int> selectedTagIds = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == id).Select(i => i.TagId).ToList();
            ItemVM itemVM = new ItemVM()
            {
                TagList = _db.Tags.AsNoTracking().Select(i => new TagListVM
                {
                    TagId = i.Id,
                    Name = i.Name,
                    CheckedState = selectedTagIds.Contains(i.Id)
                }).ToList(),
                Item = new Item(),
                ThemeSelectList = _db.Themes.AsNoTracking().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            return itemVM;
        }


        public void UpdateItemTagsList(ItemVM itemVM)
        {
            ICollection<int> ThemeTagIdsList = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == itemVM.Item.ThemeId).Select(i => i.TagId).ToList();
            ICollection<ItemTag> ItemTagsList = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == itemVM.Item.Id).ToList();
            ICollection<int> ItemTagIdsList = ItemTagsList.Select(i => i.TagId).ToList();

            if (itemVM.TagList != null)
            {
                foreach (TagListVM tag in itemVM.TagList)
                {
                    //If the checkbox is checked and the tag is available in the selected theme
                    if (tag.CheckedState && ThemeTagIdsList.Contains(tag.TagId))
                    {
                        //If the entry in the join table does not already exist
                        if (!ItemTagIdsList.Contains(tag.TagId))
                        {
                            _db.ItemTags.Add(new ItemTag
                            {
                                ItemId = itemVM.Item.Id,
                                TagId = tag.TagId
                            });
                        }
                    }
                    else
                    {
                        if (ItemTagIdsList.Contains(tag.TagId))
                        {
                            _db.ItemTags.Remove(ItemTagsList.FirstOrDefault(i => i.TagId == tag.TagId));
                        }
                    }
                }
            }
        }

        //GET - GetAssignedTags
        public ICollection<int> GetAssignedTags(int id)
        {
            ICollection<int> selectedTagIds = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == id).Select(i => i.TagId).ToList();

            ICollection<int> AssignedTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == id).Include(i => i.Tag).Select(i => i.TagId).ToList();

            return AssignedTagIds;
        }

        public void Update(Item obj)
        {
            _db.Items.Update(obj);
        }
    }
}
