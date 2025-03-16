using DataLayer.DbContext;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DbObject;

namespace RepoLayer.Implemention
{
    public class GroupRepository : BaseRepo<Group>, IGroupRepository
    {
        public GroupRepository(WeLearnContext dbContext) : base(dbContext)
        {
        }

        public override Task CreateAsync(Group entity)
        {
            return base.CreateAsync(entity);
        }

        public override async Task<Group> GetByIdAsync(int id)
        {
            return await dbContext.Groups
                //.Include(e=>e.Class)
                .Include(e=>e.GroupSubjects).ThenInclude(e=>e.Subject)

                //.Include(e => e.Schedules).ThenInclude(a => a.Meetings).ThenInclude(m => m.Chats).ThenInclude(c => c.Account)
                //.Include(e => e.Schedules).ThenInclude(e => e.Meetings).ThenInclude(m=>m.Reviews).ThenInclude(c=>c.Reviewee)
                //.Include(e => e.Schedules).ThenInclude(e => e.Meetings).ThenInclude(m=>m.Reviews).ThenInclude(c=>c.Details).ThenInclude(rd=>rd.Reviewer)
                //.Include(e=>e.Schedules) .ThenInclude(s => s.ScheduleSubjects).ThenInclude(ss => ss.Subject)

                //.Include(e => e.Schedules).ThenInclude(a => a.Meetings).ThenInclude(m => m.Chats)
                //.Include(e => e.Schedules).ThenInclude(e => e.Meetings).ThenInclude(m => m.Reviews).ThenInclude(c=>c.Details)
                //.Include(e=>e.Schedules) .ThenInclude(s => s.ScheduleSubjects)//.ThenInclude(ss => ss.Subject)

                .Include(e=>e.GroupMembers).ThenInclude(e=>e.Account)
                .Include(e => e.JoinInvites).ThenInclude(e => e.Account)
                .Include(e=>e.JoinRequests).ThenInclude(e=>e.Account)

                //.Include(e => e.Discussions).ThenInclude(e => e.AnswerDiscussion)
                //.Include(e => e.DocumentFiles)
                .Where(x => x.IsBanned != true)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e=>e.Id == id);
        }

        public override IQueryable<Group> GetList()
        {
            return base.GetList();
        }

        public override Task RemoveAsync(int id)
        {
            return base.RemoveAsync(id);
        }

        public override Task UpdateAsync(Group entity)
        {
            return base.UpdateAsync(entity);
        }
    }
}
