using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.Models.ViewModels;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
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
            Item item = new Item();
            IList<string> suggestedTagsList = new List<string>();
            IList<string> keyInsightsList = new List<string>{""};
            if (id !=null && id != 0)
            {
                if(_db.Items.FirstOrDefault(u => u.Id == id).SuggestedTags != null)
                {
                    suggestedTagsList = _db.Items.FirstOrDefault(u => u.Id == id).SuggestedTags.ToString().Split("~~");
                }
                if (_db.Items.FirstOrDefault(u => u.Id == id).KeyInsights != null)
                {
                    keyInsightsList = _db.Items.FirstOrDefault(u => u.Id == id).KeyInsights.ToString().Split("~~");
                }
                item = _db.Items.Find(id);
            }

            string[] statusInputs = {WC.Draft, WC.Submitted, WC.Published, WC.Rejected };
            IList<string> statusSelectList = new List<string>(statusInputs);

            ICollection<int> selectedTagIds = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == id).Select(i => i.TagId).ToList();
            ItemVM itemVM = new ItemVM()
            {
                SuggestedTagList = suggestedTagsList,
                KeyInsightsList = keyInsightsList,
                TagList = _db.Tags.AsNoTracking().Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = selectedTagIds.Contains(i.Id)
                }).ToList(),

                ThemeSelectList = _db.Themes.AsNoTracking().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                TeamSelectList = _db.Teams.AsNoTracking().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ProjectSelectList = _db.Projects.AsNoTracking().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                StatusSelectList = statusSelectList.Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                ApprovalRadioButtons = new List<RadioButtonVM>
                {
                    new RadioButtonVM { Value = 1, Name = WC.Internal },
                    new RadioButtonVM { Value = 2, Name = WC.External }
                },
                SensitivityRadioButtons = new List<RadioButtonVM>
                {
                    new RadioButtonVM { Value = 1, Name = WC.Unclassified },
                    new RadioButtonVM { Value = 2, Name = WC.Protected }
                },
                Item = item
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
                foreach (CheckboxVM tag in itemVM.TagList)
                {
                    //If the checkbox is checked and the tag is available in the selected theme
                    if (tag.CheckedState && ThemeTagIdsList.Contains(tag.Value))
                    {
                        //If the entry in the join table does not already exist
                        if (!ItemTagIdsList.Contains(tag.Value))
                        {
                            _db.ItemTags.Add(new ItemTag
                            {
                                ItemId = itemVM.Item.Id,
                                TagId = tag.Value
                            });
                        }
                    }
                    else
                    {
                        if (ItemTagIdsList.Contains(tag.Value))
                        {
                            _db.ItemTags.Remove(ItemTagsList.FirstOrDefault(i => i.TagId == tag.Value));
                        }
                    }
                }
            }
        }

        //GET - GetAssignedProjects
        public ItemListVM GetItemListVM(string filterType, string checkedCheckbox, string searchText = "")
        {
            IEnumerable<Item> items = _db.Items.Where(u => u.Status == WC.Published).Include(i => i.Theme);
            string[] sensitivityArray = { WC.Unclassified, WC.Protected };
            IList<string> sensitivityList = sensitivityArray.ToList();
            string[] approvalArray = { WC.Internal, WC.External };
            IList<string> approvalList = approvalArray.ToList();

            ItemListVM itemListVM = new ItemListVM
            {
                Items = items,
                SearchText = searchText,
                TagCheckboxes = _db.Tags.AsNoTracking().Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = false
                }).ToList(),
                ThemeCheckboxes = _db.Themes.AsNoTracking().Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = filterType == "theme" && checkedCheckbox == i.Name ? true : false
                }).ToList(),
                TeamCheckboxes = _db.Teams.AsNoTracking().Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = false
                }).ToList(),
                ProjectCheckboxes = _db.Projects.AsNoTracking().Select(i => new CheckboxVM
                {
                    Value = i.Id,
                    Name = i.Name,
                    CheckedState = filterType == "project" && checkedCheckbox == i.Name ? true : false
                }).ToList(),
                SensitivityCheckboxes = sensitivityList.Select(i => new CheckboxVM
                {
                    Value = sensitivityList.IndexOf(i),
                    Name = i,
                    CheckedState = false
                }).ToList(),
                ApprovalCheckboxes = approvalList.Select(i => new CheckboxVM
                {
                    Value = approvalList.IndexOf(i),
                    Name = i,
                    CheckedState = false
                }).ToList()
            };
            return itemListVM;
        }

        //GET - GetAssignedProjects
        public ICollection<int> GetAssignedProjects(IList<int> ids)
        {
            ICollection<int> AssignedProjectIds = new List<int>();
            foreach(int id in ids)
            {
                ICollection<int> newProjectIds = _db.Projects.AsNoTracking().Where(i => i.TeamId == id).Select(i => i.Id).ToList();
                foreach(int newProjectId in newProjectIds)
                {
                    if(!AssignedProjectIds.Contains(newProjectId))
                    {
                        AssignedProjectIds.Add(newProjectId);
                    }
                }
            }
            return AssignedProjectIds;
        }

        //GET - GetAssignedTags
        public ICollection<int> GetAssignedTags(IList<int> ids)
        {
            ICollection<int> AssignedTagIds = new List<int>();
            foreach (int id in ids)
            {
                ICollection<int> newTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == id).Include(i => i.Tag).Select(i => i.TagId).ToList();
                foreach (int newTagId in newTagIds)
                {
                    if (!AssignedTagIds.Contains(newTagId))
                    {
                        AssignedTagIds.Add(newTagId);
                    }
                }
            }
            return AssignedTagIds;
        }

        public void Update(Item obj)
        {
            _db.Items.Update(obj);
        }
    }
}
