using System.Linq.Expressions;
using LinqKit;
using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Services
{
    public static class PlatePedicateHelper
    {
        public static Expression<Func<Plate, bool>> GetPlateSearchPredicate(string[] searchItems)
        {
            if (!searchItems.Any())
                return PredicateBuilder.New<Plate>(true);

            var pred = PredicateBuilder.New<Plate>(false);

            foreach (var searchItem in searchItems)
            {
                pred = pred.Or(p => p.Registration == searchItem);
            }

            // RC: Include plates where the number is included inside the registration
            var fullSearch = searchItems[0];
            pred = pred.Or(p => p.Registration!.Replace(p.Numbers.ToString(), "") == fullSearch);

            // RC: Only show plates that are for sale when searching for a plate
            pred = pred.And(p => !p.IsReserved);

            return pred;
        }
    }
}