using DataLayer.DbContext;
using DataLayer.DbObject;
using RepoLayer.Interface;

namespace RepoLayer.Implemention
{
    internal class InviteReposity : BaseRepo<Invite>, IInviteReposity
    {
        public InviteReposity(WeLearnContext dbContext) : base(dbContext)
        {
        }

        public override Task CreateAsync(Invite entity)
        {
            return base.CreateAsync(entity);
        }

        public override Task<Invite> GetByIdAsync(int id)
        {
            return base.GetByIdAsync(id);
        }

        public override IQueryable<Invite> GetList()
        {
            return base.GetList();
        }

        public override Task RemoveAsync(int id)
        {
            return base.RemoveAsync(id);
        }

        public override Task UpdateAsync(Invite entity)
        {
            return base.UpdateAsync(entity);
        }
    }
}