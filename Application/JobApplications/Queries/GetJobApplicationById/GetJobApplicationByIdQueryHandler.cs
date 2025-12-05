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
        private readonly IMapper _mapper;

        public GetJobApplicationByIdQueryHandler(
            IGenericRepository<JobApplication> jobApplicationRepository,
            IMapper mapper)
        {
            _jobApplicationRepository = jobApplicationRepository;
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
            return OperationResult<JobApplicationDto>.Success(dto);
        }
    }
}
