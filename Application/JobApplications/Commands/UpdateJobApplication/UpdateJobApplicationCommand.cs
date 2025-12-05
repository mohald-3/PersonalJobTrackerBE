using Application.JobApplications.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.JobApplications.Commands.UpdateJobApplication
{

    public record UpdateJobApplicationCommand(UpdateJobApplicationDto Dto)
        : IRequest<OperationResult<JobApplicationDto>>;
}
