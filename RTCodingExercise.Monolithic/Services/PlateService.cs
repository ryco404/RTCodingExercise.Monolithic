using System.Linq.Expressions;
using System.Text.RegularExpressions;
using RTCodingExercise.Monolithic.Models;

namespace RTCodingExercise.Monolithic.Services
{
    public class PlateService : IPlateService
    {
        private readonly IEntityService _entityService;
        private readonly PlateLetterHelper _plateLetterHelper;

        private const string DefaultSortColumn = nameof(Plate.CreatedOnUtc);
        public const bool DefaultSortAscending = false;

        public PlateService(IEntityService entityService, PlateLetterHelper plateLetterHelper)
        {
            _entityService = entityService;
            _plateLetterHelper = plateLetterHelper;
        }

        public async Task<PagedList<PlateViewModel>> GetPlatesPagedAsync(int page)
        {
            Expression<Func<Plate, bool>> query = (_) => true;

            var plates = await _entityService.GetPagedEntitiesAsync(page - 1, query, DefaultSortColumn, DefaultSortAscending);
            var plateList = new PagedList<PlateViewModel>(plates);

            // RC: Sale price includes 20% markup
            plateList.Items = plates.Items!.Select(p => new PlateViewModel 
            {
                Plate = p.Registration,
                PurchasePrice = p.PurchasePrice,
                SalePrice = p.SalePrice * 1.2m
            }).ToList();

            return plateList;
        }

        public async Task SavePlateAsync(PlateViewModel plateViewModel)
        {
            var newPlate = new Plate
            {
                Id = Guid.NewGuid(),
                Registration = plateViewModel.Plate,
                PurchasePrice = plateViewModel.PurchasePrice,
                SalePrice = plateViewModel.SalePrice
            };

            var reg = newPlate.Registration!;
            var numberMatch = Regex.Match(reg, @"\d+");

            if (numberMatch.Success)
            {
                newPlate.Numbers = int.Parse(numberMatch.Value);
            }

            newPlate.Letters = _plateLetterHelper.GetLettersFromPlate(reg);

            await _entityService.SaveEntityAsync(newPlate);
        }
    }
}
