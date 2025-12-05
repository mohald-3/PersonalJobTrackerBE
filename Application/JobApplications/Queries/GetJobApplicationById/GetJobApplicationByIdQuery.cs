using Application.JobApplications.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.JobApplications.Queries.GetJobApplicationById
{
    public record GetJobApplicationByIdQuery(Guid Id) : IRequest<OperationResult<JobApplicationDto>>;
}
