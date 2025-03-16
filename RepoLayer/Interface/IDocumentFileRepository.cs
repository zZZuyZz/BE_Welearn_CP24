using DataLayer.DbObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interface
{
    public interface IDocumentFileReposity : IBaseRepo<DocumentFile>
    {
        Task<List<DocumentFile>> GetDocumentFilesByGroupId(int groupId);
        Task ApproveRejectAsync(DocumentFile entity);
        public Task UpdateRangeAsync(List<DocumentFile>? entities);
        Task<List<DocumentFile>> GetDocumentFilesByGroupIdAndAccountId(int groupId, int accountId);
    }
}
