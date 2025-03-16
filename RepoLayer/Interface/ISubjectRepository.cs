using DataLayer.DbContext;
using DataLayer.DbObject;

namespace RepoLayer.Interface
{
    public interface ISubjectRepository
    {
        IQueryable<Subject> GetList();
        Task CreateSubjectAsync(string name);
    }
}