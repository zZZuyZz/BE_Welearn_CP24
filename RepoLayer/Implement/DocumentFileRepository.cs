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
    public class DocumentFileRepository : BaseRepo<DocumentFile>, IDocumentFileReposity
    {
        private readonly WeLearnContext dbContext;

        public DocumentFileRepository(WeLearnContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public override async Task<DocumentFile> GetByIdAsync(int id)
        {
            return await dbContext.DocumentFiles
                 .Include(x => x.Account).ThenInclude(x => x.GroupMembers)
                 .FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<List<DocumentFile>> GetDocumentFilesByGroupId(int groupId)
        {
            return await dbContext.DocumentFiles
                .Include(e => e.Account)
                .Include(e => e.Group)
                .Where(x => x.GroupId == groupId && x.IsActive == true).ToListAsync();
        }

        public async Task<List<DocumentFile>> GetDocumentFilesByGroupIdAndAccountId(int groupId, int accountId)
        {
            return await dbContext.DocumentFiles
                .Include(e => e.Account)
                .Include(e => e.Group)
                .Where(x => x.GroupId == groupId && x.AccountId == accountId).ToListAsync();
        }

        public async Task UpdateRangeAsync(List<DocumentFile>? entities)
        {
            dbContext.UpdateRange(entities);
            await dbContext.SaveChangesAsync();
        }

        public Task ApproveRejectAsync(DocumentFile entity)
        {
            return base.UpdateAsync(entity);
        }

        public override Task UpdateAsync(DocumentFile entity)
        {
            return base.UpdateAsync(entity);
        }
    }
}
