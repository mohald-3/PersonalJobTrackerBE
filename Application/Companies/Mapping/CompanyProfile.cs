using Application.Companies.Dtos;
using AutoMapper;
using Domain.Models.Entities;

namespace Application.Companies.Mapping
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            // Entity -> DTO
            CreateMap<Company, CompanyDto>();

            // DTO -> Entity (Create)
            CreateMap<CreateCompanyDto, Company>();

            // DTO -> Entity (Update)
            // This will map matching properties onto an existing Company instance.
            CreateMap<UpdateCompanyDto, Company>();
        }
    }
}
