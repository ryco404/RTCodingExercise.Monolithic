using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Services
{
    public interface IPlateService 
    {
        Task<PagedList<PlateViewModel>> GetPlatesPagedAsync(int page);

        Task SavePlateAsync(PlateViewModel plateViewModel);
    }
}
