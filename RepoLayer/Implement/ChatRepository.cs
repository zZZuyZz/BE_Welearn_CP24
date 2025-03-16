using DataLayer.DbContext;
using DataLayer.DbObject;
using RepoLayer.Interface;

namespace RepoLayer.Implemention
{
    internal class ChatRepository : BaseRepo<Chat>, IChatRepository
    {
        public ChatRepository(WeLearnContext dbContext) : base(dbContext)
        {
        }

        public override Task CreateAsync(Chat entity)
        {
            return base.CreateAsync(entity);
        }

        public override Task<Chat> GetByIdAsync(int id)
        {
            return base.GetByIdAsync(id);
        }

        public override IQueryable<Chat> GetList()
        {
            return base.GetList();
        }

        public override Task RemoveAsync(int id)
        {
            return base.RemoveAsync(id);
        }

        public override Task UpdateAsync(Chat entity)
        {
            return base.UpdateAsync(entity);
        }
    }
}