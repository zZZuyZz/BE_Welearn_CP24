using DataLayer.DbContext;
using DataLayer.DbObject;
using Microsoft.EntityFrameworkCore;

namespace RepoLayer.Interface
{
    public interface IGroupMemberReposity : IBaseRepo<GroupMember> 
    {
        public Task<GroupMember> GetGroupMemberByMemberId(int id);
        public Task<List<GroupMember>> GetGroupMemberListByGroupId(int groupId);
        public Task UpdateRangeAsync(List<GroupMember> entities);
    }
}