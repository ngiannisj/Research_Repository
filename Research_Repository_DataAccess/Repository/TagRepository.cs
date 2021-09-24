using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using System.Collections.Generic;
using System.Linq;

namespace Research_Repository_DataAccess.Repository
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        private readonly ApplicationDbContext _db;

        public TagRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //Get dropdown list of all tags
        public IEnumerable<SelectListItem> GetTagList(IList<Tag> tags, bool useDb)
        {
            //If useDb parameter is true, use tags from the database to generate the tags selectlist
            if (useDb == true)
            {
                tags = _db.Tags.AsNoTracking().ToList();
            }

            //If tags parameter is null, instantiate an empty tags list
            if (tags == null)
            {
                tags = new List<Tag>();
            }

            //Generate a tags selectlist
            IEnumerable<SelectListItem> tagSelectList = tags.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return tagSelectList;
        }

        public void Update(Tag obj)
        {
            _db.Tags.Update(obj);
        }
    }
}
