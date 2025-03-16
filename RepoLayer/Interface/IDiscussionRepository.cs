using DataLayer.DbObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interface
{
    public interface IDiscussionRepository : IBaseRepo<Discussion>
    {
        Task<List<Discussion>> GetDiscussionsByGroupId(int groupId);
        Task<List<Discussion>> GetDiscussionsByGroupIdAndAccountId(int groupId, int accountId);
        public Task UpdateRangeAsync(List<Discussion>? entities);

    }
}
