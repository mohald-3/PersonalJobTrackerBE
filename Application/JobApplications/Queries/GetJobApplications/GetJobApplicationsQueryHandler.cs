using Application.Common.Interfaces;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.JobApplications.Queries.GetJobApplications
{
    public class GetJobApplicationsQueryHandler
        : IRequestHandler<GetJobApplicationsQuery, OperationResult<IEnumerable<JobApplicationDto>>>
    {
        private readonly IGenericRepository<JobApplication> _jobApplicationRepository;
        private readonly IMapper _mapper;

        public GetJobApplicationsQueryHandler(
            IGenericRepository<JobApplication> jobApplicationRepository,
            IMapper mapper)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<IEnumerable<JobApplicationDto>>> Handle(
            GetJobApplicationsQuery request,
            CancellationToken cancellationToken)
        {
            var entities = await _jobApplicationRepository.GetAllAsync();
            var dtoList = _mapper.Map<IEnumerable<JobApplicationDto>>(entities);

            return OperationResult<IEnumerable<JobApplicationDto>>.Success(dtoList);
        }
    }
}
