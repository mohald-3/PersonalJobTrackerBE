using Domain.Models.Common;
using MediatR;

namespace Application.JobApplications.Commands.DeleteJobApplication
{
    public record DeleteJobApplicationCommand(Guid Id) : IRequest<OperationResult<bool>>;
}
