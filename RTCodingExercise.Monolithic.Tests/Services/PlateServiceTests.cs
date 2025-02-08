using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using RTCodingExercise.Monolithic.Models;
using RTCodingExercise.Monolithic.Services;

namespace RTCodingExercise.Monolithic.Tests.Services
{
    [TestFixture]
    public class PlateServiceTests
    {
        private readonly PlateLetterHelper _PlateLetterHelper = new PlateLetterHelper();

        [Test]
        public void GetPlatesPagedAsync_ShouldIncludeMarkupOnSalePrice()
        {
            var mkEntitySvc = new Mock<IEntityService>();
            var salePrice = 200m;

            var plate = new Plate()
            {
                Id = Guid.NewGuid(),
                SalePrice = salePrice
            };

            mkEntitySvc.Setup(m => m.GetPagedEntitiesAsync(0, It.IsAny<Expression<Func<Plate, bool>>>(), 
                It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new PagedList<Plate>
                {
                    Items = new List<Plate> { plate }
                });
            
            var svc = new PlateService(mkEntitySvc.Object, _PlateLetterHelper);
            var result = svc.GetPlatesPagedAsync(1, null, null).Result;

            Assert.That(result.Items![0].SalePrice, Is.EqualTo(salePrice * 1.2m));
        }

        [Test]
        public void GetPlatesPagedAsync_ShouldSortByCreatedDateDescByDefault()
        {
            var mkEntitySvc = new Mock<IEntityService>();
            string? defaultSortColumn = null;
            bool? defaultIsAscending = null;

            mkEntitySvc.Setup(m => m.GetPagedEntitiesAsync(0, It.IsAny<Expression<Func<Plate, bool>>>(), 
                It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new PagedList<Plate> { Items = new() })
                .Callback<int, Expression<Func<Plate, bool>>, string, bool>((page, expr, sortByColumn, isAscending) => {
                    defaultSortColumn = sortByColumn;
                    defaultIsAscending = isAscending;
                });

            var svc = new PlateService(mkEntitySvc.Object, _PlateLetterHelper);
            var result = svc.GetPlatesPagedAsync(1, null, null).Result;

            Assert.That(defaultSortColumn, Is.EqualTo(nameof(Plate.CreatedOnUtc)));
            Assert.That(defaultIsAscending, Is.EqualTo(false));
        }

        [Test]
        public void SavePlateAsync_ShouldSetNumbersAndLettersCorrectly()
        {
            var mkEntitySvc = new Mock<IEntityService>();
            var vm = new PlateViewModel
            {
                Id = Guid.NewGuid().ToString(),
                Plate = "FA23ER"
            };
            Plate? savedPlate = null;

            mkEntitySvc.Setup(m => m.SaveEntityAsync(It.IsAny<Plate>()))
                .Callback<Plate>(p => savedPlate = p);

            var svc = new PlateService(mkEntitySvc.Object, _PlateLetterHelper);
            svc.SavePlateAsync(vm).Wait();

            Assert.That(savedPlate!.Letters, Is.EqualTo("FAZ"));
            Assert.That(savedPlate!.Numbers, Is.EqualTo(23));
        }

        [Test]
        public void SetIsReserved_ShouldThrowInvalidOperationExceptionIfPlateNotFound()
        {
            var mkEntitySvc = new Mock<IEntityService>();
            var id = Guid.NewGuid();
            var isReserved = true;

            mkEntitySvc.Setup(m => m.GetEntityAsync<Plate>(id))
                .ReturnsAsync((Plate?)null);

            var svc = new PlateService(mkEntitySvc.Object, _PlateLetterHelper);
            
            var ioex = Assert.ThrowsAsync<InvalidOperationException>(async () => await svc.SetIsReserved(id, isReserved));
            Assert.That(ioex.Message, Contains.Substring(id.ToString()));
        }
    }
}
