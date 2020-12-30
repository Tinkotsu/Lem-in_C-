using System;
using System.Collections.Generic;
using AutoMapper;
using Moq;
using WebApi.BLL.BusinessModels.Material;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Services;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Interfaces;
using Xunit;

namespace WebApi.Tests
{
    public class MaterialServiceTests
    {
        [Fact]
        public void GetMaterials_ValidCall()
        {
            //arange
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var samples = GetSampleMaterials();

            unitOfWorkMock.Setup(p => p.Materials.GetAll()).Returns(samples);

            var materialService = new MaterialService(unitOfWorkMock.Object, null);

            //act
            var result = materialService.GetMaterials();

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Material, MaterialBM>()).CreateMapper();

            var mappedSamples = mapper.Map<IEnumerable<Material>, List<MaterialBM>>(samples);
            
            //assert
            Assert.Equal(mappedSamples, result);
        }

        private IEnumerable<Material> GetSampleMaterials()
        {
            var output = new List<Material>
            {
                new Material
                {
                    Id = 1,
                    Name = "name1",
                    CategoryId = 1,
                    ActualVersionNum = 1,
                    OwnerUserId = "name1"
                },

                new Material
                {
                    Id = 2,
                    Name = "name2",
                    CategoryId = 2,
                    ActualVersionNum = 1,
                    OwnerUserId = "name2"
                },

                new Material
                {
                    Id = 3,
                    Name = "name3",
                    CategoryId = 3,
                    ActualVersionNum = 1,
                    OwnerUserId = "name3"
                },
            };
            return output;
        }

    }
}
