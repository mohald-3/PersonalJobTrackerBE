using Application.Common.Interfaces;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.JobApplications.Queries.GetJobApplicationsForCompany
{
    public class GetJobApplicationsForCompanyQueryHandler
        : IRequestHandler<GetJobApplicationsForCompanyQuery, OperationResult<IEnumerable<JobApplicationDto>>>
    {
        private readonly IGenericRepository<JobApplication> _jobApplicationRepository;
        private readonly IMapper _mapper;

        public GetJobApplicationsForCompanyQueryHandler(
            IGenericRepository<JobApplication> jobApplicationRepository,
            IMapper mapper)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<IEnumerable<JobApplicationDto>>> Handle(
            GetJobApplicationsForCompanyQuery request,
            CancellationToken cancellationToken)
        {
            var entities = await _jobApplicationRepository.FindAsync(a => a.CompanyId == request.CompanyId);

            var dtoList = _mapper.Map<IEnumerable<JobApplicationDto>>(entities);

            return OperationResult<IEnumerable<JobApplicationDto>>.Success(dtoList);
        }
    }
}
