using DataLayer.DbObject;

namespace ServiceLayer.Services.Interface.Db
{
    public interface ISubjectService
    {
        public Task CreateSubjectAsync(string name);
        IQueryable<T> GetList<T>();
        public Task<bool> IsExistAsync(int id);
    }
}