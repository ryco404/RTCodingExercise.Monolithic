using RTCodingExercise.Monolithic.Models;
using RTCodingExercise.Monolithic.Services;
using System.Diagnostics;

namespace RTCodingExercise.Monolithic.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPlateService _plateSvc;

        private bool IsAjax => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        public HomeController(IPlateService plateSvc)
        {
            _plateSvc = plateSvc;
        }

        public async Task<IActionResult> Index(int? page, bool? sortSalePriceAsc, string? search)
        {
            var plates = await _plateSvc.GetPlatesPagedAsync(page ?? 1, sortSalePriceAsc, search);

            return View(plates);
        }

        public IActionResult Add()
        {
            var vm = new PlateViewModel();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PlateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _plateSvc.SavePlateAsync(vm);

                return View("ConfirmAdd", vm);
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(string id, bool isReserved)
        {
            if (!IsAjax)
            {
                return NotFound();
            }

            bool success = true;
            string? errorMessage = null;

            try
            {
                await _plateSvc.SetIsReserved(Guid.Parse(id), isReserved);

                Log.Information("Plate reservation set to {isReserved} for Plate: {id}", isReserved, id);
            }
            catch(Exception ex)
            {
                success = false;
                errorMessage = ex.Message;

                Log.Error(ex, "Error updating plate reservation for Plate: {id}", id);
            }

            return Json(new PlateReservationResponseJson(isReserved, success, errorMessage));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
