using Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.JobApplications.Dtos
{
    public class JobApplicationDto
    {
        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!; // helpful for UI

        public string PositionTitle { get; set; } = null!;

        public ApplicationStatus Status { get; set; }

        public DateTime? AppliedDate { get; set; }
        public DateTime LastUpdated { get; set; }

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Source { get; set; }
        public int? Priority { get; set; }
        public string? Notes { get; set; }
    }
}
