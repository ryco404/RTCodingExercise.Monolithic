using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RTCodingExercise.Monolithic.Data;
using RTCodingExercise.Monolithic.Models;
using RTCodingExercise.Monolithic.Services;

namespace RTCodingExercise.Monolithic.Tests.Services
{
    [TestFixture]
    public class EntityServiceTests
    {
        private ApplicationDbContext _dbCtx;

        [SetUp]
        public void SetupMockDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("RTCodingExercise")
                .Options;

            _dbCtx = new(options);
        }

        [TearDown]
        public void DestroyMockDb()
        {
            _dbCtx.Plates.RemoveRange(_dbCtx.Plates);
            _dbCtx.SaveChanges();

            _dbCtx.Dispose();
        }

        [Test]
        public void GetPagedEntitiesAsync_ShouldOrderByColumnNameAndDirection()
        {
            var entityCount = 10;
            var entities = GetSeedData(entityCount);
            var newPlate = new Plate {
                 CreatedOnUtc = DateTime.UtcNow,
                 Registration = "RY4N"
            };
            entities.Add(newPlate);

            _dbCtx.Plates.AddRange(entities);
            _dbCtx.SaveChanges();

            var svc = GetEntityService(20);
            var result = svc.GetPagedEntitiesAsync<Plate>(0, _ => true, nameof(Plate.CreatedOnUtc), false).Result;

            Assert.That(result.Items![0], Is.EqualTo(newPlate));
        }

        [Test]
        public void GetPagedEntitiesAsync_ShouldPageCorrectly()
        {
            var entityCount = 50;
            var takeCount = 20;
            var entities = GetSeedData(entityCount);
            
            _dbCtx.Plates.AddRange(entities);
            _dbCtx.SaveChanges();

            var svc = GetEntityService(takeCount);
            // RC: Get 2nd page
            var result = svc.GetPagedEntitiesAsync<Plate>(1, _ => true, nameof(Plate.CreatedOnUtc), false).Result;

            Assert.That(result.PageCount, Is.EqualTo(entityCount / takeCount + 1));
            Assert.That(result.FirstItem, Is.EqualTo(takeCount + 1));
            Assert.That(result.LastItem, Is.EqualTo(takeCount * result!.PageNumber));
            Assert.That(result.TotalItemCount, Is.EqualTo(entityCount));
        }

        [Test]
        public void GetPagedEntitiesAsync_ShouldFilterCorrectly()
        {
            var entityCount = 5;
            var reg = "RY4N";
            var entities = GetSeedData(entityCount);
            var newPlate = new Plate {
                 CreatedOnUtc = DateTime.UtcNow,
                 Registration = reg
            };
            entities.Add(newPlate);

            _dbCtx.Plates.AddRange(entities);
            _dbCtx.SaveChanges();

            var svc = GetEntityService();
            var result = svc.GetPagedEntitiesAsync<Plate>(0, (p) => p.Registration == reg, nameof(Plate.CreatedOnUtc), false).Result;
            
            Assert.That(result.TotalItemCount, Is.EqualTo(1));
            Assert.That(result.Items![0], Is.EqualTo(newPlate));
        }

        [Test]
        public void SaveEntityAsync_ShouldAddNewEntityOnSave()
        {
            var newPlate = new Plate();

            var svc = GetEntityService();
            svc.SaveEntityAsync(newPlate).Wait();
            
            Assert.That(_dbCtx.Plates.FirstOrDefault(), Is.EqualTo(newPlate));
        }

        [Test]
        public void SaveEntityAsync_ShouldUpdateEntityIfExisting()
        {
            var entities = GetSeedData(1);

            _dbCtx.Plates.AddRange(entities);
            _dbCtx.SaveChanges();

            entities[0].IsReserved = true;

            var svc = GetEntityService();
            svc.SaveEntityAsync(entities[0]).Wait();

            Assert.That(_dbCtx.Plates.Count(), Is.EqualTo(1));
            Assert.That(_dbCtx.Plates.First().IsReserved, Is.EqualTo(true));
        }

        [TestCase("6674c2b7-7c77-4ad2-8928-f6b8925eb77d")]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        public void GetEntityAsync_ShouldReturnEntityOrNull(string id)
        {
            var entities = GetSeedData(1);
            entities[0].Id = Guid.Parse("6674c2b7-7c77-4ad2-8928-f6b8925eb77d");

            if (!_dbCtx.Plates.Any())
            {
                _dbCtx.Plates.AddRange(entities);
                _dbCtx.SaveChanges();
            }

            var svc = GetEntityService();
            var entityId = Guid.Parse(id);
            var entity = svc.GetEntityAsync<Plate>(entityId).Result;

            Assert.That(entity, Is.Null.Or.EqualTo(entities[0]));
        }

        private EntityService GetEntityService(int takeCount = 10)
        {
            var mkOpts = new Mock<IOptions<QueryOptions>>();
            mkOpts.SetupGet(m => m.Value).Returns(new QueryOptions() { TakeCount = takeCount });

            return new EntityService(mkOpts.Object, _dbCtx);
        }

        private IList<Plate> GetSeedData(int count)
        {
            var plates = new List<Plate>();
            var dt = new DateTime(2025, 1, 1);

            for(var i = 0; i < count; ++i)
            {
                var p = new Plate
                {
                    Id = Guid.NewGuid(),
                    CreatedOnUtc = dt.AddDays(i),
                    Registration = "AAAA" + ('A' + i)
                };

                plates.Add(p);
            }

            return plates;
        }
    }
}
