using DataLayer.DbContext;
using DataLayer.DbObject;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Implemention;
using RepoLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implement
{
    public class DiscussionRepository : BaseRepo<Discussion>, IDiscussionRepository
    {
        private readonly WeLearnContext dbContext;

        public DiscussionRepository(WeLearnContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public override async Task<Discussion> GetByIdAsync(int id)
        {
            return await dbContext.Discussions
                 .Include(x => x.Account)
                 .Include(x => x.AnswerDiscussion)
                 .FirstOrDefaultAsync(x => x.Id == id);
        }
        public override Task CreateAsync(Discussion entity)
        {
            return base.CreateAsync(entity);
        }

        public override Task UpdateAsync(Discussion entity)
        {
            return base.UpdateAsync(entity);
        }

        public async Task<List<Discussion>> GetDiscussionsByGroupId(int groupId)
        {
            return await dbContext.Discussions
                .Include(e => e.Account)
                .Include(e => e.Group)
                .Include(e => e.AnswerDiscussion)
                .Where(x => x.GroupId == groupId && x.IsActive != false)
                .OrderByDescending(x => x.CreateAt)
                .ToListAsync();
        }

        public async Task<List<Discussion>> GetDiscussionsByGroupIdAndAccountId(int groupId, int accountId)
        {
            return await dbContext.Discussions
                .Include(e => e.Account)
                .Include(e => e.Group)
                .Include(e => e.AnswerDiscussion)
                .Where(x => x.GroupId == groupId && x.AccountId == accountId)
                .OrderByDescending(x => x.CreateAt)
                .ToListAsync();
        }
        public async Task UpdateRangeAsync(List<Discussion>? entities)
        {
            dbContext.UpdateRange(entities);
            await dbContext.SaveChangesAsync();
        }
    }
}
