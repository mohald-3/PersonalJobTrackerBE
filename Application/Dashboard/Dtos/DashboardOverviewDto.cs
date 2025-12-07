using Application.JobApplications.Dtos;

namespace Application.Dashboard.Dtos
{
    public class DashboardOverviewDto
    {
        public int TotalCompanies { get; set; }
        public int TotalApplications { get; set; }

        public List<StatusCountDto> ApplicationsByStatus { get; set; } = new();

        // Last X days – we’ll default to 30 in the handler
        public List<JobApplicationDto> RecentApplications { get; set; } = new();

        // Top N companies by application count
        public List<CompanyApplicationsSummaryDto> TopCompaniesByApplications { get; set; } = new();
    }
}
