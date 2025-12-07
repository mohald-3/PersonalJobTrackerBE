using Application.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/dashboard/overview
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview(
            [FromQuery] int recentDays = 30,
            [FromQuery] int topCompaniesCount = 5,
            CancellationToken cancellationToken = default)
        {
            var query = new GetDashboardOverviewQuery(recentDays, topCompaniesCount);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result); // OperationResult<DashboardOverviewDto>

            return BadRequest(result);
        }
    }
}
