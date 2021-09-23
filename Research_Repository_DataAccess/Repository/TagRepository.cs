using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (useDb == true)
            {
                tags = _db.Tags.AsNoTracking().ToList();
            }

            if (tags == null)
            {
                tags = new List<Tag>();
            }

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
