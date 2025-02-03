using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Services
{
    public interface IPlateService 
    {
        Task<PagedList<PlateViewModel>> GetPlatesPagedAsync(int page, bool? sortSalePriceAsc, string? search);

        Task SavePlateAsync(PlateViewModel plateViewModel);
    }
}
