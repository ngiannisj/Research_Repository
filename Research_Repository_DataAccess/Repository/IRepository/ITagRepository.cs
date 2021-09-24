using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository_Models;
using System.Collections.Generic;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface ITagRepository : IRepository<Tag>
    {
        IEnumerable<SelectListItem> GetTagList(IList<Tag> tags, bool useDb);

        void Update(Tag obj);
    }
}
