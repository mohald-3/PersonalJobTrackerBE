using Application.JobApplications.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.JobApplications.Queries.GetJobApplications
{
    public record GetJobApplicationsQuery() : IRequest<OperationResult<IEnumerable<JobApplicationDto>>>;
}
