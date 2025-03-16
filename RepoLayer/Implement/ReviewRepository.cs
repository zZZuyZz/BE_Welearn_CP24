using DataLayer.DbContext;
using DataLayer.DbObject;
using RepoLayer.Interface;

namespace RepoLayer.Implemention
{
    internal class ReviewRepository : BaseRepo<Review>, IReviewRepository
    {
        public ReviewRepository(WeLearnContext dbContext) : base(dbContext)
        {
        }

        public override Task CreateAsync(Review entity)
        {
            return base.CreateAsync(entity);
        }

        public override Task<Review> GetByIdAsync(int id)
        {
            return base.GetByIdAsync(id);
        }

        public override IQueryable<Review> GetList()
        {
            return base.GetList();
        }

        public Task<bool> IdExistAsync(int id)
        {
            return base.IdExistAsync(id);
        }

        public override Task RemoveAsync(int id)
        {
            return base.RemoveAsync(id);
        }

        public override Task UpdateAsync(Review entity)
        {
            return base.UpdateAsync(entity);
        }
    }
}