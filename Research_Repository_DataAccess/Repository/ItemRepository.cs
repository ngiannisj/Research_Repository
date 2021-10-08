using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using Research_Repository_Models.ViewModels;
using Research_Repository_Utility;
using System.Collections.Generic;
using System.Linq;

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
            //Instantiate new item object
            Item item = new Item();
            //Instantiate new suggested tag list
            IList<string> suggestedTagsList = new List<string>();
            //Instantiate new key insights list
            IList<string> keyInsightsList = new List<string> { "" };

            //If specific item is requested, populate item object, suggested tag list and key insights list
            if (id != null && id != 0)
            {
                //Split suggested tag string from database and populate suggested tag list with it
                if (_db.Items.FirstOrDefault(u => u.Id == id).SuggestedTags != null)
                {
                    suggestedTagsList = _db.Items.FirstOrDefault(u => u.Id == id).SuggestedTags.ToString().Split("~~");
                }

                //Split key insights string from database and populate key insights list with it
                if (_db.Items.FirstOrDefault(u => u.Id == id).KeyInsights != null)
                {
                    keyInsightsList = _db.Items.FirstOrDefault(u => u.Id == id).KeyInsights.ToString().Split("~~");
                }

                //Populate item object with item from database
                item = _db.Items.AsNoTracking().Include(i => i.Project).Include(i => i.Team).Include(i => i.Theme).FirstOrDefault(u => u.Id == id);
            }

            //Instantiate status option list with all available options
            string[] statusInputs = { WC.Draft, WC.Submitted, WC.Published, WC.Rejected };
            IList<string> statusSelectList = new List<string>(statusInputs);

            //Get selected tag ids for the item from the itemTags join database table
            ICollection<int> selectedTagIds = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == id).Select(i => i.TagId).ToList();

            //Generate an item view model to pass to the view
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

        //
        public void UpdateItemTagsList(ItemVM itemVM)
        {
            //Get list of tag ids associated with the theme selected in itemVM
            ICollection<int> ThemeTagIdsList = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == itemVM.Item.ThemeId).Select(i => i.TagId).ToList();
            //Get list of itemTags for the item
            ICollection<ItemTag> ItemTagsList = _db.ItemTags.AsNoTracking().Where(i => i.ItemId == itemVM.Item.Id).ToList();
            //Get list of tag ids associated with the item in itemVM
            ICollection<int> ItemTagIdsList = ItemTagsList.Select(i => i.TagId).ToList();

            //If item tag list from itemVM has values
            if (itemVM.TagList != null && itemVM.TagList.Count > 0)
            {
                foreach (CheckboxVM tag in itemVM.TagList)
                {
                    //If the checkbox is checked and the tag is available in the selected theme
                    if (tag.CheckedState && ThemeTagIdsList.Contains(tag.Value))
                    {
                        //If the entry in the join table does not already exist
                        if (!ItemTagIdsList.Contains(tag.Value))
                        {
                            //Add entry to the itemTags database
                            _db.ItemTags.Add(new ItemTag
                            {
                                ItemId = itemVM.Item.Id,
                                TagId = tag.Value
                            });
                        }
                    }
                    else
                    {
                        //If tag checkbox is not checked or the tag is not available from the selected theme's tag list
                        if (ItemTagIdsList.Contains(tag.Value))
                        {
                            //Remove the tag from the item tag join table in the database
                            _db.ItemTags.Remove(ItemTagsList.FirstOrDefault(i => i.TagId == tag.Value));
                        }
                    }
                }
            }
        }

        //GET - ItemListVM (Currently generates filters for item list search)
        public ItemListVM GetItemListVM(string filterType, string checkedCheckbox, string searchText = "")
        {
            //Generate sensitivity checkboxes
            string[] sensitivityArray = { WC.Unclassified, WC.Protected };
            IList<string> sensitivityList = sensitivityArray.ToList();

            //Generate approval checkboxes
            string[] approvalArray = { WC.Internal, WC.External };
            IList<string> approvalList = approvalArray.ToList();

            //Generate filters for items list search
            ItemListVM itemListVM = new ItemListVM
            {
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
                    CheckedState = filterType == WC.ThemeName && checkedCheckbox == i.Name ? true : false
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
                    CheckedState = filterType == WC.ProjectName && checkedCheckbox == i.Name ? true : false
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

        //GET - GetAssignedProjects (Returns projects with a foreign key of a selected team in the team checkboxes)
        public IList<int> GetAssignedProjects(IList<int> teamIds)
        {
            //Instantiate a list of project ids
            IList<int> AssignedProjectIds = new List<int>();

            //Populate 'AssignedProjectIds' list
            foreach (int id in teamIds)
            {
                //Get ids of projects which have a foreign key of the selected teams
                IList<int> newProjectIds = _db.Projects.AsNoTracking().Where(i => i.TeamId == id).Select(i => i.Id).ToList();

                //Add project ids to 'AssignedProjectIds' list if they do not already exist in there
                foreach (int newProjectId in newProjectIds)
                {
                    if (!AssignedProjectIds.Contains(newProjectId))
                    {
                        AssignedProjectIds.Add(newProjectId);
                    }
                }
            }

            return AssignedProjectIds;
        }

        //GET - GetAssignedTags (Returns tags with a foreign key of a selected theme in the theme checkboxes)
        public IList<int> GetAssignedTags(IList<int> themeIds)
        {
            //Instantiate a list of tag ids
            IList<int> AssignedTagIds = new List<int>();

            //Populate 'AssignedTagsIds' list
            foreach (int id in themeIds)
            {
                //Get ids of tags which have a foreign key of the selected themes
                IList<int> newTagIds = _db.ThemeTags.AsNoTracking().Where(i => i.ThemeId == id).Include(i => i.Tag).Select(i => i.TagId).ToList();

                //Add tag ids to 'AssignedTagIds' list if they do not already exist in there
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
