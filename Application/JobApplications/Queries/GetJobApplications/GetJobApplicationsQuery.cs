using Application.JobApplications.Dtos;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;

namespace Application.JobApplications.Queries.GetJobApplications
{
    public record GetJobApplicationsQuery(
        int PageNumber = 1,
        int PageSize = 10,
        Guid? CompanyId = null,
        ApplicationStatus? Status = null,
        DateTime? AppliedFrom = null,
        DateTime? AppliedTo = null,
        string? Search = null
    ) : IRequest<OperationResult<PagedResult<JobApplicationDto>>>;
}
