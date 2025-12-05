using Domain.Models.Enums;

namespace Application.JobApplications.Dtos
{
    public class UpdateJobApplicationDto
    {
        public Guid Id { get; set; } // or use route param instead

        public int CompanyId { get; set; }

        public string PositionTitle { get; set; } = null!;

        public ApplicationStatus Status { get; set; }

        public DateTime? AppliedDate { get; set; }

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Source { get; set; }
        public int? Priority { get; set; }
        public string? Notes { get; set; }
    }
}
