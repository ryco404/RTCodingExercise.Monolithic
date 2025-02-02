using RTCodingExercise.Monolithic.Models;
using RTCodingExercise.Monolithic.Services;
using System.Diagnostics;

namespace RTCodingExercise.Monolithic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPlateService _plateSvc;

        public HomeController(ILogger<HomeController> logger, IPlateService plateSvc)
        {
            _logger = logger;
            _plateSvc = plateSvc;
        }

        public async Task<IActionResult> Index(int? page, bool? sortSalePriceAsc)
        {
            var plates = await _plateSvc.GetPlatesPagedAsync(page ?? 1, sortSalePriceAsc);

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
