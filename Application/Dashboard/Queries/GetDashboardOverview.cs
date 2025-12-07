using Application.Dashboard.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Dashboard.Queries
{

    public record GetDashboardOverviewQuery(
        int RecentDays = 30,
        int TopCompaniesCount = 5
    ) : IRequest<OperationResult<DashboardOverviewDto>>;
}
