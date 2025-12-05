using Application.Common.Interfaces;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.JobApplications.Commands.CreateJobApplication
{
    public class CreateJobApplicationCommandHandler
        : IRequestHandler<CreateJobApplicationCommand, OperationResult<JobApplicationDto>>
    {
        private readonly IGenericRepository<JobApplication> _jobApplicationRepository;
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public CreateJobApplicationCommandHandler(
            IGenericRepository<JobApplication> jobApplicationRepository,
            IGenericRepository<Company> companyRepository,
            IMapper mapper)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<JobApplicationDto>> Handle(
            CreateJobApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // Business rule: company must exist
            var company = await _companyRepository.GetByIdAsync(dto.CompanyId);
            if (company is null)
            {
                return OperationResult<JobApplicationDto>.Failure(
                    $"Company with id '{dto.CompanyId}' was not found.");
            }

            var entity = _mapper.Map<JobApplication>(dto);

            await _jobApplicationRepository.AddAsync(entity);

            var resultDto = _mapper.Map<JobApplicationDto>(entity);
            return OperationResult<JobApplicationDto>.Success(resultDto);
        }
    }
}
