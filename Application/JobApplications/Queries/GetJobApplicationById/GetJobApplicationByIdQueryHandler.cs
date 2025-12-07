using Application.Common.Interfaces;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.JobApplications.Queries.GetJobApplicationById
{
    public class GetJobApplicationByIdQueryHandler
        : IRequestHandler<GetJobApplicationByIdQuery, OperationResult<JobApplicationDto>>
    {
        private readonly IGenericRepository<JobApplication> _jobApplicationRepository;
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public GetJobApplicationByIdQueryHandler(
            IGenericRepository<JobApplication> jobApplicationRepository,
            IGenericRepository<Company> companyRepository,
            IMapper mapper)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<JobApplicationDto>> Handle(
            GetJobApplicationByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(request.Id);
            if (entity is null)
            {
                return OperationResult<JobApplicationDto>.Failure(
                    $"JobApplication with id '{request.Id}' was not found.");
            }
            var dto = _mapper.Map<JobApplicationDto>(entity);
            var company = await _companyRepository.GetByIdAsync(dto.CompanyId);
            dto.CompanyName = company?.Name ?? string.Empty;
            return OperationResult<JobApplicationDto>.Success(dto);
        }
    }
}
