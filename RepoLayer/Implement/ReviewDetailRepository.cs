using DataLayer.DbContext;
using DataLayer.DbObject;
using RepoLayer.Interface;

namespace RepoLayer.Implemention
{
    internal class ReviewDetailRepository : BaseRepo<ReviewDetail>, IReviewDetailRepository
    {
        public ReviewDetailRepository(WeLearnContext dbContext) : base(dbContext)
        {
        }

        public override Task CreateAsync(ReviewDetail entity)
        {
            return base.CreateAsync(entity);
        }

        public override Task<ReviewDetail> GetByIdAsync(int id)
        {
            return base.GetByIdAsync(id);
        }

        public override IQueryable<ReviewDetail> GetList()
        {
            return base.GetList();
        }

        public override Task RemoveAsync(int id)
        {
            return base.RemoveAsync(id);
        }

        public override Task UpdateAsync(ReviewDetail entity)
        {
            return base.UpdateAsync(entity);
        }
    }
}