using Application.JobApplications.Commands.CreateJobApplication;
using Application.JobApplications.Commands.DeleteJobApplication;
using Application.JobApplications.Commands.UpdateJobApplication;
using Application.JobApplications.Dtos;
using Application.JobApplications.Queries.GetJobApplicationById;
using Application.JobApplications.Queries.GetJobApplications;
using Application.JobApplications.Queries.GetJobApplicationsForCompany;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobApplicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/jobapplications
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? companyId = null,
            [FromQuery] ApplicationStatus? status = null,
            [FromQuery] DateTime? appliedFrom = null,
            [FromQuery] DateTime? appliedTo = null,
            [FromQuery] string? search = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetJobApplicationsQuery(
                pageNumber,
                pageSize,
                companyId,
                status,
                appliedFrom,
                appliedTo,
                search);

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result); // result.Data is PagedResult<JobApplicationDto>

            return BadRequest(result);
        }

        // GET: api/jobapplications/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetJobApplicationByIdQuery(id), cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            var errors = result.Errors ?? new List<string>();
            if (errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                return NotFound(result);

            return BadRequest(result);
        }

        // GET: api/jobapplications/company/{companyId}
        [HttpGet("company/{companyId:guid}")]
        public async Task<IActionResult> GetForCompany(Guid companyId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetJobApplicationsForCompanyQuery(companyId),
                cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        // POST: api/jobapplications
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateJobApplicationDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState); // your ApiBehaviorOptions will return OperationResult<string>.Failure

            var result = await _mediator.Send(
                new CreateJobApplicationCommand(dto),
                cancellationToken);

            if (!result.IsSuccess)
            {
                // Business errors / company not found etc.
                var errors = result.Errors ?? new List<string>();
                if (errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                    return NotFound(result);

                return BadRequest(result);
            }

            var created = result.Data!;
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                result); // return OperationResult<JobApplicationDto>
        }

        // PUT: api/jobapplications/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateJobApplicationDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (dto.Id == Guid.Empty)
                dto.Id = id;
            else if (dto.Id != id)
                return BadRequest(OperationResult<string>.Failure("Route id and body id do not match."));

            var result = await _mediator.Send(
                new UpdateJobApplicationCommand(dto),
                cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            var errors = result.Errors ?? new List<string>();
            if (errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                return NotFound(result);

            return BadRequest(result);
        }

        // DELETE: api/jobapplications/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new DeleteJobApplicationCommand(id),
                cancellationToken);

            if (result.IsSuccess)
                return NoContent(); // delete: 204 with no body

            var errors = result.Errors ?? new List<string>();
            if (errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                return NotFound(result);

            return BadRequest(result);
        }
    }
}
