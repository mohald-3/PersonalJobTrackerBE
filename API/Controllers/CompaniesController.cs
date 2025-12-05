using Application.Companies.Commands.CreateCompany;
using Application.Companies.Commands.DeleteCompany;
using Application.Companies.Commands.UpdateCompany;
using Application.Companies.Dtos;
using Application.Companies.Queries.GetCompanies;
using Application.Companies.Queries.GetCompanyById;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompaniesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/companies
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? city = null,
            [FromQuery] string? country = null,
            [FromQuery] string? industry = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCompaniesQuery(pageNumber, pageSize, search, city, country, industry);

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result); // result.Data is PagedResult<CompanyDto>

            return BadRequest(result);
        }

        // GET: api/companies/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            var errors = result.Errors ?? new List<string>();
            if (errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                return NotFound(result);

            return BadRequest(result);
        }

        // POST: api/companies
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCompanyDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState); // your ApiBehaviorOptions wraps as OperationResult<string>

            var result = await _mediator.Send(
                new CreateCompanyCommand(dto),
                cancellationToken);

            if (!result.IsSuccess)
            {
                var errors = result.Errors ?? new List<string>();
                if (errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                    return NotFound(result);

                return BadRequest(result);
            }

            var created = result.Data!;
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                result); // Return OperationResult<CompanyDto>
        }

        // PUT: api/companies/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCompanyDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (dto.Id == Guid.Empty)
                dto.Id = id;
            else if (dto.Id != id)
                return BadRequest(OperationResult<string>.Failure("Route id and body id do not match."));

            var result = await _mediator.Send(
                new UpdateCompanyCommand(dto),
                cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            var errors = result.Errors ?? new List<string>();
            if (errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                return NotFound(result);

            return BadRequest(result);
        }

        // DELETE: api/companies/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new DeleteCompanyCommand(id),
                cancellationToken);

            if (result.IsSuccess)
                return NoContent();

            var errors = result.Errors ?? new List<string>();
            if (errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                return NotFound(result);

            return BadRequest(result);
        }
    }
}
