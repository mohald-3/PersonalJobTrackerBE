using Application.Companies.Dtos;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Entities;

namespace Application.JobApplications.Mapping
{

    public class JobApplicationProfile : Profile
    {
        public JobApplicationProfile()
        {
            // Entity -> DTO
            CreateMap<JobApplication, JobApplicationDto>()
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : string.Empty));

            // Create DTO -> Entity
            CreateMap<CreateJobApplicationDto, JobApplication>()
                .ForMember(dest => dest.LastUpdated,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Update DTO -> Entity
            CreateMap<UpdateJobApplicationDto, JobApplication>()
                .ForMember(dest => dest.LastUpdated,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
