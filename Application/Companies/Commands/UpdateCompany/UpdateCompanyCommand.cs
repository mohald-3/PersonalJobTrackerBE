using Application.Companies.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Companies.Commands.UpdateCompany
{
    public record UpdateCompanyCommand(UpdateCompanyDto Dto) : IRequest<OperationResult<CompanyDto>>;
}
