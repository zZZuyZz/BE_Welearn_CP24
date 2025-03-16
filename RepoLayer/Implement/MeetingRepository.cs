using DataLayer.DbContext;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using DataLayer.DbObject;

namespace RepoLayer.Implemention
{
    internal class MeetingRepository : BaseRepo<Meeting>, IMeetingRepository 
    {
        public MeetingRepository(WeLearnContext context)
            : base(context)
        {
        }

        public override Task CreateAsync(Meeting entity)
        {
            return base.CreateAsync(entity);
        }

        public async Task<IEnumerable<Meeting>> MassCreateAsync(IEnumerable<Meeting> creatingMeetings)
        {
            //await dbContext.Meetings.AddRangeAsync(creatingMeetings);
            foreach (var item in creatingMeetings)
            {
                await dbContext.Meetings.AddAsync(item);
            }
            await dbContext.SaveChangesAsync();
            return creatingMeetings;
        }

        public override async Task<Meeting> GetByIdAsync(int id)
        {
            return await dbContext.Meetings
                .Include(m=>m.Schedule)
                .SingleOrDefaultAsync(m=>m.Id==id);
        }

        public override IQueryable<Meeting> GetList()
        {
            return base.GetList();
        }
        
        public override Task RemoveAsync(int id)
        {
            return base.RemoveAsync(id);
        }
        
        public async override Task UpdateAsync(Meeting entity)
        {
            dbContext.Meetings.Update(entity);
            await dbContext.SaveChangesAsync();
        }

        ///SignalR
        ////////////////////////////////////////////////////////////
        public async Task<Meeting> GetMeetingByIdSignalr(int meetingId)
        {
            return await dbContext.Meetings
                .Include(x => x.Connections)
                .Include(x => x.Schedule)
                .FirstOrDefaultAsync(x => x.Id == meetingId);
        }

        public async Task<Meeting> GetMeetingForConnectionSignalr(string connectionId)
        {
            return await dbContext.Meetings
                .Include(x => x.Connections)
                .Include(x => x.Schedule)
                .Where(x => x.Connections.Any(c => c.SinganlrId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task EndConnectionSignalr(Connection connection)
        {
            connection.End = DateTime.Now;
            dbContext.Connections.Update(connection);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateCountMemberSignalr(int roomId, int count)
        {
            var meeting = await dbContext.Meetings.FindAsync(roomId);
            if (meeting != null)
            {
                meeting.CountMember = count;
            }
            dbContext.Meetings.Update(meeting);
            await dbContext.SaveChangesAsync();
        }

        public IQueryable<Connection> GetActiveConnectionsForMeetingSignalr(int meetingId)
        {
            return dbContext.Connections
                .Where(e => e.MeetingId == meetingId && e.End == null);
        }

        public async Task EndMeetingSignalRAsync(Meeting meeting)
        {
            meeting.End = DateTime.Now;
            dbContext.Meetings.Update(meeting);
            await dbContext.SaveChangesAsync();
        }
    }
}