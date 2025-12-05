using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.JobApplications.Commands.DeleteJobApplication
{
    public class DeleteJobApplicationCommandHandler : IRequestHandler<DeleteJobApplicationCommand, OperationResult<bool>>
    {
        private readonly IGenericRepository<JobApplication> _jobApplicationRepository;

        public DeleteJobApplicationCommandHandler(
            IGenericRepository<JobApplication> jobApplicationRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
        }

        public async Task<OperationResult<bool>> Handle(
            DeleteJobApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(request.Id);
            if (entity is null)
            {
                return OperationResult<bool>.Failure(
                    $"JobApplication with id '{request.Id}' was not found.");
            }

            await _jobApplicationRepository.DeleteAsync(entity);

            return OperationResult<bool>.Success(true);
        }
    }
}
