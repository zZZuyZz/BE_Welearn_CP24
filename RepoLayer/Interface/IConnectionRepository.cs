using DataLayer.DbObject;

namespace RepoLayer.Interface
{
    public interface IConnectionRepository
    {
        public IQueryable<Connection> GetList();
        public Task<Connection> GetBySignalrIdAsync(string id);
        public Task CreateAsync(Connection entity);
        public Task UpdateAsync(Connection entity);
        public Task RemoveAsync(string id);
        public Task<bool> IdExistAsync(string id);
        public Task CreateConnectionSignalrAsync(Connection connection);
        public Task<int> CountMemberInMeeting(int meetingId);

    }
}