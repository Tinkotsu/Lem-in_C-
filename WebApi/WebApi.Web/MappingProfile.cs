using System;
using AutoMapper;
using WebApi.BLL.BusinessModels.Material;
using WebApi.DAL.Entities.Material;

namespace WebApi.Web
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Material, MaterialBm>();
            CreateMap<MaterialVersion, MaterialVersionBm>();
        }
    }
}
