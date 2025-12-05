using Application.JobApplications.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.JobApplications.Queries.GetJobApplicationsForCompany
{
    public record GetJobApplicationsForCompanyQuery(Guid CompanyId) : IRequest<OperationResult<IEnumerable<JobApplicationDto>>>;
}
