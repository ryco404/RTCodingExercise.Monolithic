using System.Linq.Expressions;
using RTCodingExercise.Monolithic.Extensions;
using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Services
{
    public class EntityService : IEntityService
    {
        private readonly QueryOptions _queryOptions;
        private readonly ApplicationDbContext _dbContext;

        public EntityService(IOptions<QueryOptions> queryOptions, ApplicationDbContext dbContext)
        {
            _queryOptions = queryOptions.Value;
            _dbContext = dbContext;
        }

        public async Task<PagedList<TEntity>> GetPagedEntitiesAsync<TEntity>(int pageIdx,
          Expression<Func<TEntity, bool>> query, 
          string sortByColumn,
          bool ascending) where TEntity : class
        {
            var dbSet = _dbContext.Set<TEntity>();
            // TODO: Create extension method for this, making sorting optional + sort direction
            var results = await dbSet.Where(query)
                    .OrderByDynamic(sortByColumn, ascending)
                    .Skip(pageIdx * _queryOptions.TakeCount)
                    .Take(_queryOptions.TakeCount)
                    .ToListAsync();

            var count = await dbSet.Where(query)
                                   .CountAsync();

            return new PagedList<TEntity>
            {
                Items = results,
                FirstItem = Math.Min((pageIdx * _queryOptions.TakeCount) + 1, count),
                LastItem = Math.Min((pageIdx + 1) * _queryOptions.TakeCount, count),
                PageCount = ((count -  1) / _queryOptions.TakeCount) + 1,
                PageNumber = pageIdx + 1,
                TotalItemCount = count
            };
        }

        public async Task SaveEntityAsync<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = _dbContext.Set<TEntity>();
            await dbSet.AddAsync(entity);

            await _dbContext.SaveChangesAsync();
        }
    }
}
