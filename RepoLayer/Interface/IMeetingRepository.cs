using DataLayer.DbContext;
using DataLayer.DbObject;

namespace RepoLayer.Interface
{
    public interface IMeetingRepository : IBaseRepo<Meeting> {
        Task<Meeting> GetMeetingByIdSignalr(int meetingId);
        Task<Meeting> GetMeetingForConnectionSignalr(string connectionId);
        Task EndConnectionSignalr(Connection connection);
        Task UpdateCountMemberSignalr(int meetingId, int count);
        IQueryable<Connection> GetActiveConnectionsForMeetingSignalr(int meetingId);
        Task EndMeetingSignalRAsync(Meeting meeting);
    }
}