using Domain.Models.Enums;

namespace Domain.Models.Entities
{

    public class JobApplication
    {
        public int Id { get; set; }

        // Foreign key and navigation
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        // Required
        public string PositionTitle { get; set; } = null!;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Planned;

        public DateTime? AppliedDate { get; set; }

        // Always set (we’ll also configure a DB default)
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Source { get; set; }
        public int? Priority { get; set; }
        public string? Notes { get; set; }
    }
}
