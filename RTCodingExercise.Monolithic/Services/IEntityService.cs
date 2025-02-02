using System.Linq.Expressions;
using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Services
{
    public interface IEntityService
    {
        Task<PagedList<TEntity>> GetPagedEntitiesAsync<TEntity>(int pageIdx, Expression<Func<TEntity, bool>> query, 
            string sortByColumn, bool ascending) where TEntity : class;

        Task SaveEntityAsync<TEntity>(TEntity entity) where TEntity : class;
    }
}
