using DataLayer.DbContext;
using DataLayer.DbObject;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using System.Security.Cryptography.X509Certificates;

namespace RepoLayer.Implemention
{
    internal class ReportRepository : BaseRepo<Report>, IReportRepository
    {
        public ReportRepository(WeLearnContext dbContext) : base(dbContext)
        {
           
        }
        public override async Task<Report> GetByIdAsync(int id)
        {
            return await dbContext.Reports
                .Include(x => x.Sender)
                .Include(x => x.Account)
                .Include(x => x.Discussion)
                .Include(x => x.File)
                .Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}