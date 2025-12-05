using Application.Companies.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Companies.Queries.GetCompanyById
{
    public record GetCompanyByIdQuery(Guid Id) : IRequest<OperationResult<CompanyDto>>;
}
