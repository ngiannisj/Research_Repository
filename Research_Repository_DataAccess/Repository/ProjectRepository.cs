using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models;

namespace Research_Repository_DataAccess.Repository
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ApplicationDbContext _db;

        public ProjectRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }

        public void Update(Project obj)
        {
            _db.Projects.Update(obj);
        }
    }
}
