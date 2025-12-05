namespace Application.Companies.Dtos
{
    public class CompanyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? OrgNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Industry { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Notes { get; set; }
    }
}
