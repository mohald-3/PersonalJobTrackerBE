using Application.Common.Interfaces;
using Application.Dashboard.Dtos;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using Domain.Models.Enums;
using MediatR;

namespace Application.Dashboard.Queries
{
    public class GetDashboardOverviewQueryHandler
        : IRequestHandler<GetDashboardOverviewQuery, OperationResult<DashboardOverviewDto>>
    {
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IGenericRepository<JobApplication> _jobApplicationRepository;
        private readonly IMapper _mapper;

        public GetDashboardOverviewQueryHandler(
            IGenericRepository<Company> companyRepository,
            IGenericRepository<JobApplication> jobApplicationRepository,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<DashboardOverviewDto>> Handle(
            GetDashboardOverviewQuery request,
            CancellationToken cancellationToken)
        {
            var recentDays = request.RecentDays <= 0 ? 30 : request.RecentDays;
            var topCompaniesCount = request.TopCompaniesCount <= 0 ? 5 : request.TopCompaniesCount;

            var companies = (await _companyRepository.GetAllAsync()).ToList();
            var applications = (await _jobApplicationRepository.GetAllAsync()).ToList();

            var now = DateTime.UtcNow;
            var recentFromDate = now.AddDays(-recentDays);

            // 1) Totals
            var totalCompanies = companies.Count;
            var totalApplications = applications.Count;

            // 2) Applications by status
            var statusCounts = Enum.GetValues(typeof(ApplicationStatus))
                .Cast<ApplicationStatus>()
                .Select(status => new StatusCountDto
                {
                    Status = status,
                    Count = applications.Count(a => a.Status == status)
                })
                .ToList();

            // 3) Recent applications (last X days), newest first
            var recentApplicationsEntities = applications
                .Where(a => a.AppliedDate.HasValue && a.AppliedDate.Value >= recentFromDate)
                .OrderByDescending(a => a.AppliedDate)
                .Take(20) // cap list for dashboard
                .ToList();

            var recentApplicationsDtos = _mapper.Map<List<JobApplicationDto>>(recentApplicationsEntities);

            // Fill CompanyName manually (we already have this pattern)
            var companyDict = companies.ToDictionary(c => c.Id, c => c.Name);
            foreach (var dto in recentApplicationsDtos)
            {
                if (companyDict.TryGetValue(dto.CompanyId, out var name))
                    dto.CompanyName = name;
            }

            // 4) Top companies by application count
            var topCompanies = applications
                .GroupBy(a => a.CompanyId)
                .Select(g => new
                {
                    CompanyId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(topCompaniesCount)
                .ToList();

            var topCompaniesSummaries = topCompanies
                .Select(x =>
                {
                    companyDict.TryGetValue(x.CompanyId, out var name);
                    return new CompanyApplicationsSummaryDto
                    {
                        CompanyId = x.CompanyId,
                        CompanyName = name ?? string.Empty,
                        ApplicationsCount = x.Count
                    };
                })
                .ToList();

            var overview = new DashboardOverviewDto
            {
                TotalCompanies = totalCompanies,
                TotalApplications = totalApplications,
                ApplicationsByStatus = statusCounts,
                RecentApplications = recentApplicationsDtos,
                TopCompaniesByApplications = topCompaniesSummaries
            };

            return OperationResult<DashboardOverviewDto>.Success(overview);
        }
    }
}
