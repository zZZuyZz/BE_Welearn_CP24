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
    public class AnswerDiscussionRepository : BaseRepo<AnswerDiscussion>, IAnswerDiscussionRepository
    {
        private readonly WeLearnContext dbContext;

        public AnswerDiscussionRepository(WeLearnContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public override Task<AnswerDiscussion> GetByIdAsync(int id)
        {
            return base.GetByIdAsync(id);
        }
        public override Task CreateAsync(AnswerDiscussion entity)
        {
            return base.CreateAsync(entity);
        }
        public async Task<List<AnswerDiscussion>> GetAnswerDiscussionsByDiscussionId(int discussionId)
        {
            return await dbContext.AnswerDiscussions
                .Include(e => e.Account)
                .Where(x => x.DiscussionId == discussionId).ToListAsync();
        }
    }
}
