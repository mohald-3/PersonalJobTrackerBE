using Application.JobApplications.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.JobApplications.Commands.CreateJobApplication
{

    public record CreateJobApplicationCommand(CreateJobApplicationDto Dto) : IRequest<OperationResult<JobApplicationDto>>;
}
