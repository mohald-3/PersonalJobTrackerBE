using Domain.Models.Enums;

namespace Application.Dashboard.Dtos
{
    public class StatusCountDto
    {
        public ApplicationStatus Status { get; set; }
        public int Count { get; set; }
    }
}
