using Microsoft.AspNetCore.Mvc.Rendering;
using Research_Repository_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface ITagRepository : IRepository<Tag>
    {
        IEnumerable<SelectListItem> GetTagList(IList<Tag> tags, bool useDb);
        void Update(Tag obj);
    }
}
