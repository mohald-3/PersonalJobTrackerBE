using Application.Companies.Dtos;
using Domain.Models.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Queries.GetCompanies
{
    public record GetCompaniesQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? Search = null,
        string? City = null,
        string? Country = null,
        string? Industry = null
    ) : IRequest<OperationResult<PagedResult<CompanyDto>>>;
}
