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
        private static IEnumerable<Material> GetSampleMaterials()
        {
            var output = new List<Material>
            {
                new Material
                {
                    Id = 1,
                    Name = "name1",
                    Category = MaterialCategories.Presentation,
                    ActualVersionNum = 1,
                    OwnerUserId = "name1"
                },

                new Material
                {
                    Id = 2,
                    Name = "name2",
                    Category = MaterialCategories.Application,
                    ActualVersionNum = 1,
                    OwnerUserId = "name2"
                },

                new Material
                {
                    Id = 3,
                    Name = "name3",
                    Category = MaterialCategories.Other,
                    ActualVersionNum = 1,
                    OwnerUserId = "name3"
                },
            };
            return output;
        }

    }
}
