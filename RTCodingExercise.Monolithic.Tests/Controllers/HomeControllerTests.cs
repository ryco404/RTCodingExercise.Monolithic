using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using RTCodingExercise.Monolithic.Controllers;
using RTCodingExercise.Monolithic.Models;
using RTCodingExercise.Monolithic.Services;

namespace RTCodingExercise.Monolithic.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void Index_WithoutPageInQueryShouldFetchFirstPage()
        {
            var mkSvc = new Mock<IPlateService>();
            int? actualPage = null;

            mkSvc.Setup(m => m.GetPlatesPagedAsync(It.IsAny<int>(), null, null))
                 .Callback<int, bool?, string>((page, _, _) => {
                    actualPage = page;
                 });

            var ctrl = new HomeController(mkSvc.Object);
            ctrl.Index(null, null, null).Wait();

            Assert.That(actualPage, Is.EqualTo(1));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Add_ShouldOnlySaveToServiceIfNoModelErrors(bool hasErrors)
        {
            var mkSvc = new Mock<IPlateService>();
            var ctrl = new HomeController(mkSvc.Object);
            
            if (hasErrors)
            {
                ctrl.ModelState.AddModelError("foo", "bar");
            }

            var vm = new PlateViewModel();
            ctrl.Add(vm).Wait();

            mkSvc.Verify(p => p.SavePlateAsync(vm), Times.Exactly(hasErrors ? 0 : 1));
        }

        [Test]
        public void Reserve_ShowReturn404ForNonAjaxRequests()
        {
            var mkSvc = new Mock<IPlateService>();
            var ctrl = new HomeController(mkSvc.Object);
            SetupRequestWithHeaders(ctrl, false);

            var result = ctrl.Reserve(string.Empty, false).Result;

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void Reserve_ShouldUpdatePlateReservedAgainstIfAjaxRequest()
        {
            var mkSvc = new Mock<IPlateService>();
            var ctrl = new HomeController(mkSvc.Object);
            var id = Guid.NewGuid().ToString();
            var isReserved = true;

            SetupRequestWithHeaders(ctrl, true);

            var result = ctrl.Reserve(id, isReserved).Result;
            var jsonResult = result as JsonResult;
            var reservationJson = jsonResult!.Value as PlateReservationResponseJson;

            mkSvc.Verify(m => m.SetIsReserved(Guid.Parse(id), isReserved), Times.Once());

            Assert.That(reservationJson!.success, Is.True);
            Assert.That(reservationJson!.isReserved, Is.EqualTo(isReserved));
        }

        [Test]
        public void Reserve_ShouldReturnErrorMessageInJson()
        {
            var mkSvc = new Mock<IPlateService>();
            var ctrl = new HomeController(mkSvc.Object);
            var errorMessage = "Unexpected error occurred";
            var id = Guid.NewGuid().ToString();
            var isReserved = true;

            mkSvc.Setup(m => m.SetIsReserved(Guid.Parse(id), isReserved))
                .Throws(new Exception(errorMessage));

            SetupRequestWithHeaders(ctrl, true);

            var result = ctrl.Reserve(id, isReserved).Result;
            var jsonResult = result as JsonResult;
            var reservationJson = jsonResult!.Value as PlateReservationResponseJson;

            Assert.That(reservationJson!.success, Is.False);
            Assert.That(reservationJson!.errorMessage, Is.EqualTo(errorMessage));
        }

        private void SetupRequestWithHeaders(HomeController ctrl, bool isAjaxRequest)
        {
            const string RequestedWithAjax = "XMLHttpRequest";

            var cc = new ControllerContext();
            var defaultHttpCtx = new DefaultHttpContext();

            cc.HttpContext = defaultHttpCtx;
            ctrl.ControllerContext = cc;

            defaultHttpCtx.Request.Headers["X-Requested-With"] = isAjaxRequest 
                ? RequestedWithAjax : StringValues.Empty;
        }
    }
}
