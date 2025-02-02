using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Services
{
    public interface IPlateService 
    {
        Task<PagedList<PlateViewModel>> GetPlatesPagedAsync(int page, bool? sortSalePriceAsc);

        Task SavePlateAsync(PlateViewModel plateViewModel);
    }
}
