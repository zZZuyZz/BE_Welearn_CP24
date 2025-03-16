using DataLayer.DbContext;
using DataLayer.DbObject;
using RepoLayer.Interface;

namespace RepoLayer.Implemention
{
    internal class SubjectRepository : ISubjectRepository
    {
        private WeLearnContext dbContext;

        public SubjectRepository(WeLearnContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateSubjectAsync(string name)
        {
            await dbContext.Subjects.AddAsync(new Subject { Name = name }); 
            await dbContext.SaveChangesAsync();
        }

        public IQueryable<Subject> GetList()
        {
           return dbContext.Subjects.AsQueryable();
        }
    }
}