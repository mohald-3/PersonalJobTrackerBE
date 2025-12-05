using Domain.Models.Enums;

namespace Application.JobApplications.Dtos
{

    public class CreateJobApplicationDto
    {
        public Guid CompanyId { get; set; }

        public string PositionTitle { get; set; } = null!;

        // Optional: if omitted, handler can default to ApplicationStatus.Planned
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Planned;

        public DateTime? AppliedDate { get; set; }

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Source { get; set; }
        public int? Priority { get; set; }
        public string? Notes { get; set; }
    }
}
