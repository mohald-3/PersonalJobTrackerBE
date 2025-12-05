using static System.Net.Mime.MediaTypeNames;

namespace Domain.Models.Entities
{
    public class Company
    {
        public int Id { get; set; }

        // Required
        public string Name { get; set; } = null!;

        public string? OrgNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Industry { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Notes { get; set; }

        // Navigation property
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
