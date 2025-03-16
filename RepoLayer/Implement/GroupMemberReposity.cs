using DataLayer.DbContext;
using DataLayer.DbContext;
using DataLayer.DbObject;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;

namespace RepoLayer.Implemention
{
    public class GroupMemberReposity : BaseRepo<GroupMember>, IGroupMemberReposity
    {
        private readonly WeLearnContext _dbContext;

        public GroupMemberReposity(WeLearnContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task CreateAsync(GroupMember entity)
        {
            return base.CreateAsync(entity);
        }

        public override async Task<GroupMember> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<GroupMember?> GetGroupMemberByMemberId(int id)
        {
            return await _dbContext.GroupMembers
                        .Include(c => c.Account)
                        .Include(c => c.Group)
                        .Where(x => x.AccountId == id).FirstOrDefaultAsync();
        }

        public async Task<List<GroupMember>> GetGroupMemberListByGroupId(int groupId)
        {
            return await _dbContext.GroupMembers
                        .Include(c => c.Account)
                        .Include(c => c.Group)
                        .Where(x => x.GroupId == groupId).ToListAsync();
        }

        public override IQueryable<GroupMember> GetList()
        {
            return base.GetList();
        }

        public override Task RemoveAsync(int id)
        {
            return base.RemoveAsync(id);
        }

        public override Task UpdateAsync(GroupMember entity)
        {
            return base.UpdateAsync(entity);
        }

        public async Task UpdateRangeAsync(List<GroupMember> entities)
        {
            _dbContext.UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }
    }
}