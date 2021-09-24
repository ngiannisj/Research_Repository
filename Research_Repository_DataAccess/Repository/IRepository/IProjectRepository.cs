using Research_Repository_Models;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface IProjectRepository : IRepository<Project>
    {
        void Update(Project obj);
    }
}
