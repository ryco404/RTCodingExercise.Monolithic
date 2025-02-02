using System.Linq.Expressions;

namespace RTCodingExercise.Monolithic.Extensions
{
    public static class IQueryableExtensions
    {
        // RC: I had to get AI's assistance with this one :P - I've used dynamic expressions in the past
        // but was not previously familiar with the Expression.Query() method
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string orderByProperty, bool ascending)
        {
            var entityType = typeof(T);
            var property = entityType.GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new ArgumentException($"Property '{orderByProperty}' does not exist on type '{entityType.Name}'.");
            }

            var parameter = Expression.Parameter(entityType, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            
            var orderByMethod = ascending ? "OrderBy" : "OrderByDescending";
            
            var resultExpression = Expression.Call(
                typeof(Queryable),
                orderByMethod,
                new Type[] { entityType, property.PropertyType },
                source.Expression,
                Expression.Quote(orderByExpression));
            
            return source.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
